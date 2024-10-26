using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camaras : MonoBehaviour
{
    private List<Camera> camaras = new List<Camera>();
    //private List de puertas de trigger para las camaras (hacer)
    private Camera camJugador;
    private Camera camActual;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        obtenerCamaras();
        GameObject fpsController = GameObject.FindWithTag("Player");
        if (fpsController != null)
        {
            camJugador = fpsController.GetComponentInChildren<Camera>();
            player = fpsController;
        }
        
        if (camJugador != null)
        {
            camJugador.gameObject.SetActive(true);
            camActual = camJugador;
        }
        else
        {
            Debug.LogWarning("No se encontró una cámara en el objeto FPSController. Asegúrate de que el objeto FPSController tenga una cámara.");
        }
        foreach (Camera cam in camaras) {
            if (cam != camJugador) {
                cam.gameObject.SetActive(false);
            }
        }
        camActual = camJugador;
    }

    private void obtenerCamaras() 
    {
        camaras.AddRange(Object.FindObjectsOfType<Camera>());
        if (camaras.Count == 0)
        {
            Debug.LogWarning("No se encontraron cámaras en la escena.");
        }
    }

    public void cambiarCamara()
    {
        Camera camCercana = null;
        float menorDist = float.MaxValue;
        Vector3 posJugador = transform.position;

        foreach(Camera cam in camaras)
        {
            if (cam == camJugador) continue;
            float distacia = Vector3.Distance(posJugador,cam.transform.position);
            if (distacia < menorDist) {
                menorDist = distacia;
                camCercana = cam;
            }
        }

        if (camCercana != null && camCercana != camActual) {
            camActual.gameObject.SetActive(false);
            camCercana.gameObject.SetActive(true);
            camActual = camCercana;
            player.SetActive(false);
        }
    }

    public void volverMainCam()
    {
        if (camActual != camJugador) {
            camActual.gameObject.SetActive(false);
            camJugador.gameObject.SetActive(true);
            camActual = camJugador;
            player.SetActive(true);
        }
    }
}
