using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [Header("Refs")]
    public MazeRateTilt mazeTilt;   // o MazeRateTilt, según el que uses
    public Rigidbody ballRb;
    public Transform ballSpawn;

    [Header("UI Panels")]
    public GameObject panelMenu;
    public GameObject panelInstructions;
    public GameObject inGameMenuButton; // <- arrastrás el botón "Menu" acá

    private bool started;

    private void Start()
    {
        started = false;

        panelMenu.SetActive(true);
        panelInstructions.SetActive(false);
        if (inGameMenuButton != null) inGameMenuButton.SetActive(false);

        // pelota congelada en menú
        if (ballRb != null) ballRb.isKinematic = true;

        // tiempo normal
        Time.timeScale = 1f;
    }

    public void OnPlayPressed()
    {
        // recalibrar siempre que arrancás una run (sirve si el jugador volvió al menú)
        mazeTilt.CalibrateCenter();

        // soltar física
        if (ballRb != null) ballRb.isKinematic = false;

        panelMenu.SetActive(false);
        panelInstructions.SetActive(false);
        if (inGameMenuButton != null) inGameMenuButton.SetActive(true);

        started = true;
        Time.timeScale = 1f;
    }

    public void OnMenuPressed()
    {
        panelMenu.SetActive(true);
        panelInstructions.SetActive(false);
        inGameMenuButton.SetActive(false);

        // resetear física
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;

        // mover al spawn
        ballRb.transform.position = ballSpawn.position;
        ballRb.transform.rotation = ballSpawn.rotation;

        // congelar
        ballRb.isKinematic = true;

        started = false;
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