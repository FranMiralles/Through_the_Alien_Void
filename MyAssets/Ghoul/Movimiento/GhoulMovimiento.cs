using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhoulMovimiento : MonoBehaviour
{
    public Transform[] puntosPatrulla;
    private float velocidad = 6f;
    public NavMeshAgent navAgent;
    private float tiempoEspera = 5f;

    private int indiceActual = 0;
    private bool esperando = false;

    private Animator animator;

    public Transform jugador; // Referencia al jugador
    private float rangoDeteccion = 10f; // Distancia a la que detecta al jugador
    private float anguloVision = 140f; // Ángulo del campo de visión del enemigo
    private bool persiguiendo = false;

    private float distanciaAtaque = 3f; // Distancia a la que ataca al jugador
    private bool atacando = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        navAgent.speed = velocidad;

        if (puntosPatrulla.Length > 0)
        {
            navAgent.SetDestination(puntosPatrulla[indiceActual].position);
        }
    }

    void Update()
    {
        if (!persiguiendo && !esperando && navAgent.remainingDistance < 0.5f && !navAgent.pathPending)
        {
            StartCoroutine(EsperarYPasarAlSiguiente());
        }

        if ((JugadorEnCampoDeVision() && JugadorVisible()) || persiguiendo)
        {
            StartCoroutine(PerseguirOAtacar());
        }
    }

    bool JugadorEnCampoDeVision()
    {
        float distanciaAlJugador = Vector3.Distance(transform.position, jugador.position);
        if (distanciaAlJugador > rangoDeteccion)
        {
            return false;
        }

        Vector3 direccionAlJugador = (jugador.position - transform.position).normalized;
        float anguloEntreEnemigoYJugador = Vector3.Angle(transform.forward, direccionAlJugador);
        return anguloEntreEnemigoYJugador < anguloVision / 2f;
    }

    bool JugadorVisible()
    {
        Vector3 direccionAlJugador = (jugador.position - transform.position).normalized;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, direccionAlJugador, out hit, rangoDeteccion))
        {
            if (hit.transform.gameObject.tag == "Player")
            {
                return true;
            }
        }

        return false;
    }

    IEnumerator EsperarYPasarAlSiguiente()
    {
        esperando = true;
        animator.SetBool("run", false);
        navAgent.stoppingDistance = 0;

        indiceActual = (indiceActual + 1) % puntosPatrulla.Length;

        Vector3 direccion = puntosPatrulla[indiceActual].position - transform.position;
        Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);

        float tiempoRotacion = 0f;
        float duracionRotacion = 1f;

        while (tiempoRotacion < duracionRotacion)
        {
            tiempoRotacion += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, tiempoRotacion / duracionRotacion);
            yield return null;
        }

        transform.rotation = rotacionObjetivo;

        yield return new WaitForSeconds(tiempoEspera);
        if (!persiguiendo)
        {
            animator.SetBool("run", true);
            navAgent.SetDestination(puntosPatrulla[indiceActual].position);
            esperando = false;
        }
    }

    IEnumerator PerseguirOAtacar()
    {
        persiguiendo = true;
        animator.SetBool("run", true);
        navAgent.stoppingDistance = distanciaAtaque - 1;

        float distanciaAlJugador = Vector3.Distance(transform.position, jugador.position);

        if (distanciaAlJugador <= distanciaAtaque && !atacando)
        {
            StartCoroutine(Atacar());
        }
        else
        {
            navAgent.SetDestination(jugador.position);
        }

        yield return null;
    }

    IEnumerator Atacar()
    {
        atacando = true;
        navAgent.isStopped = true; // Detener al enemigo mientras ataca
        

        // Activar animación de ataque
        int ataqueAleatorio = Random.Range(0, 2);
        if (ataqueAleatorio == 0) { animator.SetTrigger("attack1"); }
        else { animator.SetTrigger("attack2"); }

        // Duración del ataque
        yield return new WaitForSeconds(1.1f);
        animator.SetBool("run", false);

        // Calcular la rotación hacia el jugador
        Vector3 direccion = jugador.position - transform.position;
        Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);

        // Calcular el ángulo de rotación necesario
        float angulo = Quaternion.Angle(transform.rotation, rotacionObjetivo);
        if(angulo > 150)
        {
            // Calcular la duración del giro en función del ángulo (0.9s para 360º)
            float duracionRotacion = (angulo / 360f) * 0.9f;

            float tiempoRotacion = 0f;
            while (tiempoRotacion < duracionRotacion)
            {
                tiempoRotacion += Time.deltaTime;
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, tiempoRotacion / duracionRotacion);
                yield return null;
            }
            transform.rotation = rotacionObjetivo;
        }

        // Después de atacar, seguir persiguiendo al jugador
        navAgent.SetDestination(jugador.position);
        animator.SetBool("run", true);
        navAgent.isStopped = false;
        atacando = false;
    }
}
