using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objeto : MonoBehaviour
{
    public Inventario inventario; // Referencia al script Inventario
    public Sprite iconoItem; // Icono del objeto a a�adir

    private void Start()
    {
        // Creamos un �tem de ejemplo
        ItemInventario nuevoItem = new ItemInventario("Llave", iconoItem);

        // A�adimos este �tem al inventario
        inventario.AddToInventory(nuevoItem);
    }
}
