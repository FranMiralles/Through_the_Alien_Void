using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Objeto : MonoBehaviour
{
    private Inventario inventario; // Referencia al script Inventario
    public Sprite iconoItem; // Icono del objeto a a�adir
    public string nombreItem;
    private bool jugadorCerca = false; // Controla si el jugador est� cerca del objeto
    private static Text TextoRecoger; // Texto global, est�tico para toda la escena

    private void Start()
    {
        inventario = GameObject.FindWithTag("Player").GetComponent<Inventario>();
        // Si no se ha asignado, busca el objeto de texto en la escena.
        if (TextoRecoger == null)
        {
            TextoRecoger = GameObject.FindGameObjectWithTag("TextoRecoger").GetComponent<Text>();
        }

        // Aseg�rate de que el texto est� desactivado al inicio.
        if (TextoRecoger != null)
        {
            TextoRecoger. enabled = false; // Inicialmente desactivado
        }
    }


    public void Recoger()
    {
        // Creamos un �tem con el nombre e icono del objeto.
        ItemInventario nuevoItem = new ItemInventario(nombreItem, iconoItem);

        if (inventario.AddToInventory(nuevoItem)) 
        {
            // Desactiva el texto al recoger el objeto.
            if (TextoRecoger != null)
            {
                TextoRecoger.enabled = false; // Desactiva el texto despu�s de recoger.
                //Debug.Log("Objeto recogido, texto desactivado.");
            }

            // Destruir el objeto del mundo para simular que lo hemos cogido.
            Destroy(gameObject);
        }
    }
}
