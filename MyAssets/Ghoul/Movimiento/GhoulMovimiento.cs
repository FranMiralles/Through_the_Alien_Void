using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhoulMovimiento : MonoBehaviour
{
    public Transform[] puntosPatrulla;
    private float velocidad = 5.5f;
    private NavMeshAgent navAgent;
    private float tiempoEspera = 8f;

    private int indiceActual = 0;
    private bool esperando = false;

    private Animator animator;

    public Transform jugador; // Referencia al jugador
    private float rangoDeteccion = 10f; // Distancia a la que detecta al jugador
    private float anguloVision = 140f; // Ángulo del campo de visión del enemigo
    private bool persiguiendo = false;

    private float distanciaAtaque = 3f; // Distancia a la que ataca al jugador
    private bool atacando = false;

    private bool enPuntoDeEspera = false;

    // Referencia a las corutinas y flags para activarlas luego de interrumpirlas
    private Coroutine EsperarYPasarAlSiguienteRef;
    private Coroutine PerseguirOAtacarRef;
    private Coroutine GirarHaciaJugadorRef;

    void Start()
    {
        animator = GetComponent<Animator>();
        navAgent = GetComponent < NavMeshAgent>();
        navAgent.speed = velocidad;

        if (puntosPatrulla.Length > 0)
        {
            navAgent.SetDestination(puntosPatrulla[indiceActual].position);
        }
    }

    void Update()
    {
        if (!enPuntoDeEspera && !persiguiendo && !esperando && navAgent.remainingDistance < 0.5f && !navAgent.pathPending)
        {
            EsperarYPasarAlSiguienteRef = StartCoroutine(EsperarYPasarAlSiguiente());
        }

        if (!enPuntoDeEspera && ((JugadorEnCampoDeVision() && JugadorVisible()) || persiguiendo))
        {
            PerseguirOAtacarRef = StartCoroutine(PerseguirOAtacar());
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
        if (!persiguiendo && !enPuntoDeEspera)
        {
            animator.SetBool("run", true);
            navAgent.SetDestination(puntosPatrulla[indiceActual].position);
            esperando = false;
        }
    }

    IEnumerator PerseguirOAtacar()
    {
        // En caso de ser alterado, que no ataque
        if (enPuntoDeEspera) yield break;

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

    IEnumerator GirarHaciaObjetivo(Transform objetivo, float velocidadGiro)
    {
        while (true)
        {
            // Calcula la dirección y la rotación objetivo hacia el jugador
            Vector3 direccion = objetivo.position - transform.position;
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);

            // Calcula el ángulo de diferencia
            float angulo = Quaternion.Angle(transform.rotation, rotacionObjetivo);

            // Si el ángulo es suficientemente pequeño, detiene la corrutina
            if (angulo < 20f)
                yield break;
            
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadGiro * Time.deltaTime);

            yield return null;
        }
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
        if(!enPuntoDeEspera) GirarHaciaJugadorRef = StartCoroutine(GirarHaciaObjetivo(jugador, 5f));
        yield return new WaitForSeconds(1.2f);
        animator.SetBool("run", false);

        // Después de atacar, seguir persiguiendo al jugador
        navAgent.isStopped = false;
        navAgent.SetDestination(jugador.position);
        atacando = false;
    }

    // Lanzador de la corutina mediante otro script
    public void DistraerLlamada(Transform puntoEspera)
    {
        enPuntoDeEspera = true;
        StartCoroutine(EsperarFinDeAtaque(puntoEspera));
    }

    private IEnumerator EsperarFinDeAtaque(Transform puntoEspera)
    {
        // Espera a que la corutina de atacar termine
        while (atacando)
        {
            yield return null; // Espera un frame
        }

        // Ahora puedes comenzar la distracción
        if (GirarHaciaJugadorRef != null) StopCoroutine(GirarHaciaJugadorRef);
        if (EsperarYPasarAlSiguienteRef != null) StopCoroutine(EsperarYPasarAlSiguienteRef);
        if (PerseguirOAtacarRef != null) StopCoroutine(PerseguirOAtacarRef);
        StartCoroutine(Distraer(puntoEspera));
    }

    IEnumerator Distraer(Transform puntoDeEspera)
    {
        animator.SetBool("run", true);

        StartCoroutine(GirarHaciaObjetivo(puntoDeEspera, 120f));

        navAgent.SetDestination(puntoDeEspera.position);
        navAgent.stoppingDistance = 2;

        while (Vector3.Distance(transform.position, puntoDeEspera.position) >= 2.1f)
        {
            yield return null;
        }

        animator.SetBool("run", false);
        yield return new WaitForSeconds(15f);

        enPuntoDeEspera = false;
        navAgent.stoppingDistance = 0;

        // Volver a patrullar
        animator.SetBool("run", true);
        navAgent.SetDestination(puntosPatrulla[indiceActual].position);
        esperando = false;
        persiguiendo = false;
    }
}