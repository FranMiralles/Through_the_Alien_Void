using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objeto : MonoBehaviour
{
    public Inventario inventario; // Referencia al script Inventario
    public Sprite iconoItem; // Icono del objeto a añadir

    private void Start()
    {
        // Creamos un ítem de ejemplo
        ItemInventario nuevoItem = new ItemInventario("Llave", iconoItem);

        // Añadimos este ítem al inventario
        inventario.AddToInventory(nuevoItem);
    }
}
