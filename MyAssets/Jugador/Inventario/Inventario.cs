using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventario : MonoBehaviour
{
    private List<ItemInventario> inventraio;
    public List<Image> slots;

    private void Start()
    {
        inventraio = new List<ItemInventario>();
        ActualizarInventraio();

        foreach (Image slot in slots) 
        {
            slot.enabled = true;
        }
    }

    public void AddToInventory(ItemInventario newItem) 
    {
        ItemInventario existe = inventraio.Find(item => item.nombre == newItem.nombre);

        if (existe == null)
        {
            if (inventraio.Count < 8)
            {
                inventraio.Add(newItem);
                ActualizarInventraio();
            }
            else 
            {
                Debug.Log("Invetario lleno");
            }
        }
    }

    private void ActualizarInventraio() 
    {
        for (int i = 0; i < slots.Count; i++) 
        {
            if (i < inventraio.Count)
            {
                slots[i].sprite = inventraio[i].icono;
                slots[i].enabled = true;
            }
            else
            {
                slots[i].sprite = null;
                slots[i].enabled = false;
            }
        }
    
    }
}


public class ItemInventario 
{
    public string nombre;
    public Sprite icono;

    public ItemInventario(string nombre, Sprite icono) 
    { 
        this.nombre = nombre;
        this.icono = icono; 
    }
}