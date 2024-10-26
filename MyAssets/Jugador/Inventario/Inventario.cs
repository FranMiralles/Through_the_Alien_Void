using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventario : MonoBehaviour
{
    private List<ItemInventario> inventraio;
    public List<Image> slots;
    //public Color colorSeleccionado = Color.white; // Color para el borde de selección
    public Color colorNoSeleccionado = Color.clear; // Color para los slots no seleccionados
    private int indexSeleccionado = 0; // Empezar con el primer item seleccionado

    // Lista de objetos stackeables 
    public List<string> objetosStackeables = new List<string> { "Medicamento" };

    // Referencia al texto de "Inventario lleno"
    public TextMeshProUGUI textoLleno;  

    private void Start()
    {
        inventraio = new List<ItemInventario>();
        ActualizarInventraio();

        // Asegurarse de que el texto de "Inventario lleno" esté desactivado al inicio
        
        textoLleno.gameObject.SetActive(false);
        

        foreach (Image slot in slots)
        {
            slot.enabled = true;
        }
    }

    public bool AddToInventory(ItemInventario newItem)
    {
        // Comprobar si el inventario está lleno
        //Debug.Log(inventraio.Count);
        if (inventraio.Count == 8)
        {
            //Debug.Log("Inventario lleno");
            // Activar el texto de "Inventario lleno" durante 2 segundos
            StartCoroutine(MostrarTextoLleno());
            
            return false; // Salir de la función si el inventario está lleno
        }

        // Comprobar si el objeto ya existe en el inventario
        ItemInventario existe = inventraio.Find(item => item.nombre == newItem.nombre);

        if (existe == null)
        {
            // Si no existe y hay espacio, añadirlo como nuevo objeto
            inventraio.Add(newItem);
            //Debug.Log(newItem.nombre + " añadido al inventario.");
        }
        else
        {
            // Si ya existe y es un objeto stackeable, incrementar la cantidad
            if (EsStackeable(newItem.nombre))
            {
                existe.cantidad += newItem.cantidad;
                //Debug.Log(newItem.nombre + " stackeado. Cantidad actual: " + existe.cantidad);
            }
            else
            {
                //Debug.Log("El objeto ya está en el inventario y no es stackeable.");
            }
        }

        // Actualizamos la interfaz del inventario para reflejar el cambio de cantidad
        ActualizarInventraio();
        return true;
    }

    // Método para verificar si el objeto es stackeable
    private bool EsStackeable(string nombreObjeto)
    {
        return objetosStackeables.Contains(nombreObjeto);
    }

    public void Update()
    {
        CambiarSeleccion();
    }

    private void CambiarSeleccion()
    {
        if (inventraio.Count == 0)
        {
            indexSeleccionado = -1; // No hay selección si el inventario está vacío
            ActualizarInventraio();
            return;
        }

        // Cambiar selección con la rueda del ratón
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            // Mover hacia la derecha
            indexSeleccionado++;
            if (indexSeleccionado >= inventraio.Count) indexSeleccionado = 0; // Circular
            ActualizarInventraio();
        }
        else if (scroll < 0f)
        {
            // Mover hacia la izquierda
            indexSeleccionado--;
            if (indexSeleccionado < 0) indexSeleccionado = inventraio.Count - 1; // Circular
            ActualizarInventraio();
        }

        //ActualizarInventraio();
    }

    private void ActualizarInventraio()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < inventraio.Count)
            {
                // Mostrar el icono del objeto en el slot
                slots[i].sprite = inventraio[i].icono;
                slots[i].enabled = true; // Activa el slot cuando tiene un ítem

                // Obtener el componente Outline del slot
                Outline outline = slots[i].GetComponent<Outline>();

                // Establecer el borde solo si el slot está seleccionado
                outline.enabled = i == indexSeleccionado;

                // Obtener el componente TextMeshProUGUI asociado al slot para mostrar la cantidad
                Transform cantidadTextoTransform = slots[i].transform.Find("Cantidad");
                if (cantidadTextoTransform != null)
                {
                    TextMeshProUGUI cantidadTexto = cantidadTextoTransform.GetComponent<TextMeshProUGUI>();

                    // Actualizar la cantidad siempre que el objeto sea stackeable
                    // y su cantidad sea mayor que 1
                    if (EsStackeable(inventraio[i].nombre))
                    {
                        cantidadTexto.text = inventraio[i].cantidad.ToString();
                        cantidadTexto.gameObject.SetActive(true); // Asegurar que el texto esté activado
                        //Debug.Log("Mostrando cantidad: " + inventraio[i].cantidad + " en slot " + i);
                    }
                    else
                    {
                        cantidadTexto.gameObject.SetActive(false); // Desactivar el texto si no es necesario
                        //Debug.Log("Ocultando cantidad en slot " + i);
                    }
                }
                else
                {
                    //Debug.LogWarning("No se encontró el hijo 'Cantidad' en el slot " + i);
                }
            }
            else
            {
                // Si no hay ítem en el slot, desactivar el slot
                slots[i].sprite = null; // Mostrar el slot vacío
                slots[i].enabled = false; // Desactivar la imagen del slot vacío

                // Asegurarse de que el Outline esté desactivado
                Outline outline = slots[i].GetComponent<Outline>();
                if (outline != null)
                {
                    outline.enabled = false; // Desactivar el borde en slots vacíos
                }
            }
        }
    }

    // Corrutina para mostrar el mensaje de "Inventario lleno" 
    private IEnumerator MostrarTextoLleno()
    {
        textoLleno.gameObject.SetActive(true); // Mostrar el texto
        yield return new WaitForSeconds(1); // Esperar 1 segundo
        textoLleno.gameObject.SetActive(false); // Ocultar el texto
    }
}

public class ItemInventario
{
    public string nombre;
    public Sprite icono;
    public int cantidad;

    public ItemInventario(string nombre, Sprite icono)
    {
        this.nombre = nombre;
        this.icono = icono;
        this.cantidad = 1;
    }
}
