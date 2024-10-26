using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Objeto : MonoBehaviour
{
    public Inventario inventario; // Referencia al script Inventario
    public Sprite iconoItem; // Icono del objeto a añadir
    public string nombreItem = "Llave";
    private bool jugadorCerca = false; // Controla si el jugador está cerca del objeto
    private static TextMeshProUGUI TextoRecoger; // Texto global, estático para toda la escena

    private void Start()
    {
        // Si no se ha asignado, busca el objeto de texto en la escena.
        if (TextoRecoger == null)
        {
            TextoRecoger = GameObject.FindGameObjectWithTag("TextoRecoger").GetComponent<TextMeshProUGUI>();
        }

        // Asegúrate de que el texto esté desactivado al inicio.
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
            jugadorCerca = true; // Indica que el jugador está cerca.

            if (TextoRecoger != null)
            {
                TextoRecoger.text = "Pulsa E para recoger " + nombreItem; // Cambia el texto según el objeto
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
        // Solo ejecuta el siguiente código si el jugador está cerca
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            Recoger(); // Llama a la función Recoger cuando el jugador presiona E.
        }
    }

    private void Recoger()
    {
        // Creamos un ítem con el nombre e icono del objeto.
        ItemInventario nuevoItem = new ItemInventario(nombreItem, iconoItem);

        if (inventario.AddToInventory(nuevoItem)) 
        {
            // Desactiva el texto al recoger el objeto.
            if (TextoRecoger != null)
            {
                TextoRecoger.gameObject.SetActive(false); // Desactiva el texto después de recoger.
                //Debug.Log("Objeto recogido, texto desactivado.");
            }

            // Destruir el objeto del mundo para simular que lo hemos cogido.
            Destroy(gameObject);
        }
    }
}
