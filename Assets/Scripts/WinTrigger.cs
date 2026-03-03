using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    public GameObject winText; // opcional

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("WIN!");
        if (winText != null) winText.SetActive(true);
        // para prototipo: podés pausar
        Time.timeScale = 0f;
    }
}