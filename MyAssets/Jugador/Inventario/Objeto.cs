using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Objeto : MonoBehaviour
{
    private Inventario inventario; // Referencia al script Inventario
    public Sprite iconoItem; // Icono del objeto a a�adir
    public string nombreItem = "Llave";
    private bool jugadorCerca = false; // Controla si el jugador est� cerca del objeto
    private static TextMeshProUGUI TextoRecoger; // Texto global, est�tico para toda la escena

    private void Start()
    {
        inventario = GameObject.FindWithTag("Player").GetComponent<Inventario>();
        // Si no se ha asignado, busca el objeto de texto en la escena.
        if (TextoRecoger == null)
        {
            TextoRecoger = GameObject.FindGameObjectWithTag("TextoRecoger").GetComponent<TextMeshProUGUI>();
        }

        // Aseg�rate de que el texto est� desactivado al inicio.
        if (TextoRecoger != null)
        {
            TextoRecoger.gameObject.SetActive(false); // Inicialmente desactivado
            //Debug.Log("TextoRecoger encontrado y desactivado.");
        }
        else
        {
            //Debug.LogWarning("TextoRecoger no encontrado.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true; // Indica que el jugador est� cerca.

            if (TextoRecoger != null)
            {
                TextoRecoger.text = "Pulsa E para recoger " + nombreItem; // Cambia el texto seg�n el objeto
                TextoRecoger.gameObject.SetActive(true); // Activa el texto cuando el jugador entra.
                //Debug.Log("Jugador cerca, texto activado.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false; // Indica que el jugador ha salido.

            if (TextoRecoger != null)
            {
                TextoRecoger.gameObject.SetActive(false); // Desactiva el texto cuando el jugador sale.
                //Debug.Log("Jugador lejos, texto desactivado.");
            }
        }
    }

    private void Update()
    {
        // Solo ejecuta el siguiente c�digo si el jugador est� cerca
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            Recoger(); // Llama a la funci�n Recoger cuando el jugador presiona E.
        }
    }

    private void Recoger()
    {
        // Creamos un �tem con el nombre e icono del objeto.
        ItemInventario nuevoItem = new ItemInventario(nombreItem, iconoItem);

        if (inventario.AddToInventory(nuevoItem)) 
        {
            // Desactiva el texto al recoger el objeto.
            if (TextoRecoger != null)
            {
                TextoRecoger.gameObject.SetActive(false); // Desactiva el texto despu�s de recoger.
                //Debug.Log("Objeto recogido, texto desactivado.");
            }

            // Destruir el objeto del mundo para simular que lo hemos cogido.
            Destroy(gameObject);
        }
    }
}
