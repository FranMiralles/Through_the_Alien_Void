using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhoulMovimiento : MonoBehaviour
{
    public Transform[] puntosPatrulla;
    public float velocidad;
    public NavMeshAgent navAgent;
    public float tiempoEspera = 5f;

    private int indiceActual = 0;
    private bool esperando = false;

    private Animator animator;

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
            // Cuando llegue al punto actual, inicia la espera
            Debug.Log("Cuando llegue al punto actual, inicia la espera");
            StartCoroutine(EsperarYPasarAlSiguiente());
        }
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
            tiempoRotacion +=  Time.deltaTime;
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