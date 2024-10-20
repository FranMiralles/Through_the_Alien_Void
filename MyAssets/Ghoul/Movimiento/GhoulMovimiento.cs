using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhoulMovimiento : MonoBehaviour
{
    public Transform[] puntosPatrulla;
    public float velocidad;
    public NavMeshAgent navAgent;
    private float tiempoEspera = 5f;

    private int indiceActual = 0;
    private bool esperando = false;

    private Animator animator;

    public Transform jugador; // Referencia al jugador
    private float rangoDeteccion = 12f; // Distancia a la que detecta al jugador
    private float anguloVision = 120f; // Ángulo del campo de visión del enemigo

    void Start()
    {
        animator = GetComponent<Animator>();
        // Inicializa la velocidad del NavMeshAgent
        navAgent.speed = velocidad;

        if (puntosPatrulla.Length > 0)
        {
            // Ir al primer punto de patrullaje
            navAgent.SetDestination(puntosPatrulla[indiceActual].position);
        }
    }

    void Update()
    {
        if (!esperando && navAgent.remainingDistance < 0.5f && !navAgent.pathPending)
        {
            StartCoroutine(EsperarYPasarAlSiguiente());
        }

        if(JugadorEnCampoDeVision() && JugadorVisible())
        {
            Debug.Log("Atacar");
        }
    }

    bool JugadorEnCampoDeVision()
    {
        // 1. Verificar si el jugador está dentro del rango de detección
        float distanciaAlJugador = Vector3.Distance(transform.position, jugador.position);
        if (distanciaAlJugador > rangoDeteccion)
        {
            return false; // Está demasiado lejos
        }

        // 2. Calcular la dirección hacia el jugador
        Vector3 direccionAlJugador = (jugador.position - transform.position).normalized;

        // 3. Calcular el ángulo entre la dirección hacia el jugador y la dirección en la que mira el enemigo
        float anguloEntreEnemigoYJugador = Vector3.Angle(transform.forward, direccionAlJugador);

        // 4. Si el jugador está dentro del ángulo de visión, se considera visible
        return anguloEntreEnemigoYJugador < anguloVision / 2f;
    }

    bool JugadorVisible()
    {
        // Realizar un Raycast hacia el jugador para verificar si hay algún obstáculo en el camino
        Vector3 direccionAlJugador = (jugador.position - transform.position).normalized;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, direccionAlJugador, out hit, rangoDeteccion))
        {
            // Si el Raycast golpea al jugador
            if (hit.transform.gameObject.tag == "Player")
            {
                return true; // El jugador es visible
            }
        }

        return false; // Hay algo bloqueando la visión
    }

    IEnumerator EsperarYPasarAlSiguiente()
    {
        esperando = true;
        animator.SetBool("run", false);

        // Cambiar al siguiente punto de patrullaje (antes de rotar)
        indiceActual = (indiceActual + 1) % puntosPatrulla.Length;

        // Calcula la dirección hacia el próximo punto
        Vector3 direccion = puntosPatrulla[indiceActual].position - transform.position;
        Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);

        // Tiempo para interpolar la rotación
        float tiempoRotacion = 0f;
        float duracionRotacion = 1f; // Duración total de la rotación en segundos

        // Girar hacia el próximo punto de manera progresiva
        while (tiempoRotacion < duracionRotacion)
        {
            tiempoRotacion += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, tiempoRotacion / duracionRotacion);
            yield return null; // Esperar hasta el siguiente frame
        }

        // Asegurarse de que la rotación final es precisa
        transform.rotation = rotacionObjetivo;

        // Espera durante el tiempo especificado
        yield return new WaitForSeconds(tiempoEspera);

        // Moverse al siguiente punto de patrullaje
        animator.SetBool("run", true);
        navAgent.SetDestination(puntosPatrulla[indiceActual].position);

        esperando = false; // Termina la espera
    }
}