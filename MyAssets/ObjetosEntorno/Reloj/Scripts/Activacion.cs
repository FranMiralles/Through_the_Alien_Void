using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Activacion : MonoBehaviour
{
    public Text timerText;
    private float countdownTime = 145;
    private bool isRunning = false;
    private Collider soundRange;

    private void Start()
    {
        soundRange = this.GetComponent<BoxCollider>();
        soundRange.enabled = false;
        StartTimer();
    }

    public void StartTimer()
    {
        if (!isRunning)
        {
            timerText.gameObject.SetActive(true);
            StartCoroutine(CountdownCoroutine());
        }
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
        soundRange.enabled = true;
        yield return new WaitForSeconds(1f);
        soundRange.enabled = false;
        timerText.gameObject.SetActive(false);
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
