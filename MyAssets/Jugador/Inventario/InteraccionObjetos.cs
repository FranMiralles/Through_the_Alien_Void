using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteraccionObjetos : MonoBehaviour
{
    private List<Objeto> scriptsObjeto;
    private bool objetoEnRango = false;
    private static Text TextoRecoger;

    private void Start()
    {
        TextoRecoger = GameObject.FindGameObjectWithTag("TextoRecoger").GetComponent<Text>();
        scriptsObjeto = new List<Objeto>();
        if (TextoRecoger != null)
        {
            TextoRecoger.enabled = false;
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && objetoEnRango)
        {
            foreach (Objeto objeto in scriptsObjeto)
            {
                objeto.Recoger();
            }
            scriptsObjeto.Clear();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Objeto"))
        {
            Objeto script = other.gameObject.GetComponent<Objeto>();
            objetoEnRango = true;

            if (script != null)
            {
                scriptsObjeto.Add(script);
            }

            if (TextoRecoger != null)
            {
                TextoRecoger.text = "Pulsa E para recoger "+ script.nombreItem;
                TextoRecoger.enabled = true;
            }
        }

        if (other.CompareTag("Reloj"))
        {
            if (TextoRecoger != null)
            {
                if (other.gameObject.GetComponentInChildren<Activacion>().isRunning)
                {
                    TextoRecoger.text = "Reloj en funcionamiento";
                }
                else
                {
                    TextoRecoger.text = "Inserta Pila para hacerlo funcionar";
                }
                
                TextoRecoger.enabled = true;
            }
        }

        if (other.CompareTag("Puerta"))
        {
            if (TextoRecoger != null)
            {
                if (!other.gameObject.GetComponent<AbrirPuerta>().abierta)
                {
                    TextoRecoger.text = "Cerrado";
                    TextoRecoger.enabled = true;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Objeto"))
        {
            Objeto script = other.GetComponent<Objeto>();
            objetoEnRango = false;

            if (script != null)
            {
                scriptsObjeto.Remove(script);
            }

            if (TextoRecoger != null)
            {
                TextoRecoger.enabled = false;
            }
        }

        if (other.CompareTag("Reloj"))
        {
            if (TextoRecoger != null)
            {
                TextoRecoger.enabled = false;
            }
        }

        if (other.CompareTag("Puerta"))
        {
            if (TextoRecoger != null)
            {
                TextoRecoger.enabled = false;
            }
        }
    }
}
