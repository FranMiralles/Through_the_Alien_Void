using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventario : MonoBehaviour
{
    private List<ItemInventario> inventario;
    public List<Image> slots;
    //public Color colorSeleccionado = Color.white; // Color para el borde de selección
    public Color colorNoSeleccionado = Color.clear; // Color para los slots no seleccionados
    private int indexSeleccionado = 0; // Empezar con el primer item seleccionado

    // Lista de objetos stackeables 
    private List<string> objetosStackeables = new List<string> { "Pila", "Medicamento" };

    // Referencia al texto de "Inventario lleno"
    public Text textoLleno;  

    private void Start()
    {
        inventario = new List<ItemInventario>();
        ActualizarInventario();

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
        if (inventario.Count == 8)
        {
            // Activar el texto de "Inventario lleno" durante 2 segundos
            StartCoroutine(MostrarTextoLleno());
            
            return false; // Salir de la función si el inventario está lleno
        }

        // Comprobar si el objeto ya existe en el inventario
        ItemInventario existe = inventario.Find(item => item.nombre == newItem.nombre);

        if (existe == null)
        {
            // Si no existe y hay espacio, añadirlo como nuevo objeto
            inventario.Add(newItem);
        }
        else
        {
            // Si ya existe y es un objeto stackeable, incrementar la cantidad
            if (EsStackeable(newItem.nombre))
            {
                existe.cantidad += newItem.cantidad;
            }
        }

        // Actualizamos la interfaz del inventario para reflejar el cambio de cantidad
        ActualizarInventario();
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
        if (Input.GetButtonDown("Fire1"))
        {
            if (indexSeleccionado != -1)
            {
                bool eliminar = inventario[indexSeleccionado].UsarItem(this.gameObject);
                if (eliminar) {
                    inventario.RemoveAt(indexSeleccionado);
                    ActualizarInventario();
                }
            }
        }
    }

    private void CambiarSeleccion()
    {
        if (inventario.Count == 0)
        {
            indexSeleccionado = -1; // No hay selección si el inventario está vacío
            ActualizarInventario();
            return;
        }

        if (inventario.Count == 1)
        {
            indexSeleccionado = 0; // No hay selección si el inventario está vacío
            ActualizarInventario();
            return;
        }

        // Cambiar selección con la rueda del ratón
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            // Mover hacia la derecha
            indexSeleccionado++;
            if (indexSeleccionado >= inventario.Count) indexSeleccionado = 0; // Circular
            ActualizarInventario();
        }
        else if (scroll < 0f)
        {
            // Mover hacia la izquierda
            indexSeleccionado--;
            if (indexSeleccionado < 0) indexSeleccionado = inventario.Count - 1; // Circular
            ActualizarInventario();
        }
    }

    private void ActualizarInventario()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            TextMeshProUGUI cantidadTexto = slots[i].transform.Find("Cantidad").GetComponent<TextMeshProUGUI>();
            if (i < inventario.Count)
            {
                // Mostrar el icono del objeto en el slot
                slots[i].sprite = inventario[i].icono;
                slots[i].enabled = true; // Activa el slot cuando tiene un ítem

                // Obtener el componente Outline del slot
                Outline outline = slots[i].GetComponent<Outline>();

                // Establecer el borde solo si el slot está seleccionado
                outline.enabled = i == indexSeleccionado;

                // Obtener el componente TextMeshswdaProUGUI asociado al slot para mostrar la cantidad
                if (cantidadTexto != null)
                {
                    cantidadTexto.enabled = true;
                    // Actualizar la cantidad siempre que el objeto sea stackeable
                    // y su cantidad sea mayor que 1
                    if (EsStackeable(inventario[i].nombre))
                    {
                        cantidadTexto.text = inventario[i].cantidad.ToString();
                        cantidadTexto.gameObject.SetActive(true); // Asegurar que el texto esté activado
                    }
                    else
                    {
                        cantidadTexto.gameObject.SetActive(false); // Desactivar el texto si no es necesario
                    }
                }
            }
            else
            {
                // Si no hay ítem en el slot, desactivar el slot
                slots[i].sprite = null; // Mostrar el slot vacío
                slots[i].enabled = false; // Desactivar la imagen del slot vacío
                cantidadTexto.enabled = false;
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

    public bool UsarItem(GameObject inventario)
    {
        bool consumir = false;
        if (nombre == "Medicamento")
        {
            inventario.gameObject.GetComponent<Vida>().heal(50);
            consumir = true;
        }
        if (nombre == "Pila")
        {
            // Buscar todos los objetos con el script Activacion en el rango del jugador
            Collider[] colliders = Physics.OverlapSphere(inventario.transform.position, 2f); // 5f es el rango de acción

            foreach (var collider in colliders)
            {
                if(collider.gameObject.transform.parent != null && collider.gameObject.transform.parent.CompareTag("Reloj"))
                {
                    Activacion[] activacionScripts = collider.gameObject.transform.parent.GetComponentsInChildren<Activacion>();
                    foreach (var activacionScript in activacionScripts)
                    {
                        if (activacionScript != null)
                        {
                            activacionScript.StartTimer();
                            consumir = true;
                        }
                    }
                }
            }
        }
        if(consumir) this.cantidad -= 1;
        if (this.cantidad == 0) return true;
        return false;
    }
}
