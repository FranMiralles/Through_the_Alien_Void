using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtaqueGhoul : MonoBehaviour
{
    private int contador = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            contador += 1;
            Vida vidaScript = other.gameObject.GetComponent<Vida>();
            vidaScript.damage(25);
        }
    }
}
