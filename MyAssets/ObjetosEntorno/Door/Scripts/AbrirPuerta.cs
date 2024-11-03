using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbrirPuerta : MonoBehaviour
{
    private Animator animator;
    public bool abierta;
    private Transform player;
    private Transform puntoCierre;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        puntoCierre = transform.Find("Cierre");
        abierta = false;
    }

    public void OpenDoor()
    {
        abierta = true;
        animator.SetTrigger("Open");
        StartCoroutine(CheckDistanceCoroutine());
    }

    public void CloseDoor()
    {
        abierta = false;
        animator.SetTrigger("Close");
        StopCoroutine(CheckDistanceCoroutine());
    }

    private IEnumerator CheckDistanceCoroutine()
    {
        while (abierta)
        {
            float distanciaJugador = Vector3.Distance(player.position, puntoCierre.position);
            float distanciaPuerta = Vector3.Distance(transform.position, puntoCierre.position);

            if (distanciaJugador * 2 < distanciaPuerta)
            {
                CloseDoor();
                yield break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}