using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Activacion : MonoBehaviour
{
    private Text timerText;
    private float countdownTime = 15;
    public bool isRunning = false;
    private Collider soundRange;

    private void Start()
    {
        soundRange = this.GetComponent<BoxCollider>();
        timerText = GameObject.FindGameObjectWithTag("ContadorReloj").GetComponent<Text>();
        soundRange.enabled = false;
    }

    public bool StartTimer()
    {
        if (!isRunning)
        {
            timerText.enabled = true;
            StartCoroutine(CountdownCoroutine());
        }
        return isRunning;
    }

    private IEnumerator CountdownCoroutine()
    {
        isRunning = true;
        while (countdownTime > 0)
        {
            // Muestra el tiempo en pantalla en formato minutos:segundos
            timerText.text = $"{Mathf.Floor(countdownTime / 60):00}:{Mathf.Floor(countdownTime % 60):00}";

            countdownTime -= Time.deltaTime;
            yield return null;
        }

        timerText.text = "00:00";
        isRunning = false;
        countdownTime = 15;
        soundRange.enabled = true;
        yield return new WaitForSeconds(1f);
        soundRange.enabled = false;
        timerText.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto que entra en el trigger es el enemigo y llama al método
        if (other.gameObject.tag == "Ghoul")
        {
            GhoulMovimiento ghoulMovimientoScript = other.transform.parent?.parent?.GetComponent<GhoulMovimiento>();
            if(ghoulMovimientoScript != null) ghoulMovimientoScript.DistraerLlamada(this.transform);
        }
    }
}
