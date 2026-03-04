using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [Header("Refs")]
    public MazeRateTilt mazeTilt;   // o MazeRateTilt, seg?n el que uses
    public Rigidbody ballRb;
    public Transform ballSpawn;

    [Header("UI Panels")]
    public GameObject panelMenu;
    public GameObject panelInstructions;
    public GameObject inGameMenuButton; // <- arrastr?s el bot?n "Menu" ac?

    private bool started;


    private void Start()
    {
        started = false;

        panelMenu.SetActive(true);
        panelInstructions.SetActive(false);
        if (inGameMenuButton != null) inGameMenuButton.SetActive(false);

        // pelota congelada en men?
        if (ballRb != null) ballRb.isKinematic = true;

        // tiempo normal
        Time.timeScale = 1f;
    }

    public void OnPlayPressed()
    {
        // recalibrar siempre que arranc?s una run (sirve si el jugador volvi? al men?)
        mazeTilt.CalibrateCenterForPlay();

        // soltar f?sica
        if (ballRb != null) ballRb.isKinematic = false;

        panelMenu.SetActive(false);
        panelInstructions.SetActive(false);
        if (inGameMenuButton != null) inGameMenuButton.SetActive(true);

        started = true;
        Time.timeScale = 1f;
    }

    public void OnMenuPressed()
    {
        Time.timeScale = 1f; // por si quedó pausado
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnInstructionsPressed()
    {
        panelInstructions.SetActive(true);
    }

    public void OnBackFromInstructions()
    {
        panelInstructions.SetActive(false);
    }

    public void OnExitPressed()
    {
        Application.Quit();
        Debug.Log("Exit pressed.");
    }
}