using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objeto : MonoBehaviour
{
    public Inventario inventario; // Referencia al script Inventario
    public Sprite iconoItem; // Icono del objeto a añadir
    public string nombreItem = "Llave";
    private bool jugadorCerca = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            jugadorCerca = true;
        }
    }




    private void Recoger()
    {
        // Creamos un ítem con el nombre e icono del objeto
        ItemInventario nuevoItem = new ItemInventario(nombreItem, iconoItem);

        // Añadimos este ítem al inventario
        inventario.AddToInventory(nuevoItem);

        //Destruir el objeto del mundo para simular que lo hemos cogido
        Destroy(gameObject);
    }
}
