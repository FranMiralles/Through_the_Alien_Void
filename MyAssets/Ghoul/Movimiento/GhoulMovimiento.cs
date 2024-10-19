using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhoulMovimiento : MonoBehaviour
{
    public Transform objetivo;
    public float velocidad;
    public NavMeshAgent navAgent;

   

    void Update()
    {
        navAgent.speed = velocidad;
        navAgent.SetDestination(objetivo.position);
    }
}