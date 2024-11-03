using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Cerradura : MonoBehaviour
{
    public Image barraProgreso;            // Imagen que se llenará verticalmente
    private float velocidadInicial = 1.0f;         // Velocidad de cambio de llenado
    public float velocidad;
    public AbrirPuerta abrirPuerta;        // Referencia al script de la puerta

    private bool enJuego = false;          // Controla si el minijuego está activo
    private bool subiendo = true;          // Controla la dirección de llenado
    public int faseActual = 0;            // Fase en la que se encuentra el minijuego
    private CharController playerMovement;

    // Definición de los rangos para cada fase
    private readonly Vector2[] rangos = {
        new Vector2(0.2f, 0.8f),           // Rango para la fase 1
        new Vector2(0.35f, 0.65f),         // Rango para la fase 2
        new Vector2(0.45f, 0.55f)          // Rango para la fase 3
    };

    private void Start()
    {
        // Configura la barra para que se llene de abajo a arriba
        barraProgreso.fillAmount = 0;                      // Inicia vacía
        barraProgreso.gameObject.SetActive(false);         // Oculta la barra al inicio
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<CharController>();
    }

    public void IniciarMinijuego()
    {
        playerMovement.enabled = false;
        velocidad = velocidadInicial;
        enJuego = true;
        faseActual = 0;                                    // Inicia en la fase 1
        barraProgreso.fillAmount = 0;                      // Reinicia el llenado de la barra
        subiendo = true;                                   // Inicia en dirección ascendente
        barraProgreso.gameObject.SetActive(true);          // Muestra la barra
        StartCoroutine(MinijuegoCoroutine());              // Inicia la corrutina del minijuego
    }

    private IEnumerator MinijuegoCoroutine()
    {
        while (enJuego)
        {
            // Controla el movimiento de la barra (subida o bajada)
            if (subiendo)
            {
                barraProgreso.fillAmount += velocidad * Time.deltaTime;
                if (barraProgreso.fillAmount >= 1)
                {
                    barraProgreso.fillAmount = 1;
                    subiendo = false; // Cambia la dirección a bajada
                }
            }
            else
            {
                barraProgreso.fillAmount -= velocidad * Time.deltaTime;
                if (barraProgreso.fillAmount <= 0)
                {
                    barraProgreso.fillAmount = 0;
                    subiendo = true; // Cambia la dirección a subida
                }
            }

            // Detecta si el jugador presiona la tecla de acción (barra espaciadora)
            if (Input.GetKeyDown(KeyCode.F))
            {
                VerificarAcierto(barraProgreso.fillAmount);
            }

            // Espera un pequeño intervalo antes de la siguiente verificación
            yield return new WaitForSeconds(0.005f);
        }
    }

    private void VerificarAcierto(float value)
    {
        // Verifica si la barra está en el rango correcto para la fase actual
        Vector2 rangoActual = rangos[faseActual];
        if (value >= rangoActual.x && value <= rangoActual.y)
        {
            faseActual++;
            velocidad *= 1.2f;
            if (faseActual >= rangos.Length)
            {
                // Si ha completado las tres fases, el minijuego ha sido exitoso
                playerMovement.enabled = true;
                enJuego = false;
                barraProgreso.gameObject.SetActive(false); // Oculta la barra
                abrirPuerta.OpenDoor();                    // Llama a la función para abrir la puerta
            }
        }
        else
        {
            // Reinicia el minijuego si falla
            ResetearMinijuego();
        }
    }

    private void ResetearMinijuego()
    {
        velocidad = velocidadInicial;
        barraProgreso.fillAmount = 0;
        subiendo = true;                  // Reinicia la dirección
        faseActual = 0;                   // Reinicia a la fase inicial
    }
}
