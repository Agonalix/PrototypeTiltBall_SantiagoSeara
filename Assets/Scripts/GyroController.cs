using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroController : MonoBehaviour
{
    private bool gyroEnabled;
    private Gyroscope gyro;

    private Quaternion rotFix;
    private Quaternion initialRotation;

    void Start()
    {
        // Activar giroscopio si est� disponible
        gyroEnabled = SystemInfo.supportsGyroscope;

        if (gyroEnabled)
        {
            gyro = Input.gyro;
            gyro.enabled = true;

            // Correcci�n para que la rotaci�n sea coherente con Unity
            rotFix = new Quaternion(0, 0, 1, 0);
            initialRotation = transform.rotation;
        }
        else
        {
            Debug.LogWarning("Este dispositivo no tiene giroscopio.");
        }
    }

    void Update()
    {
        if (gyroEnabled)
        {
            // Aplica rotaci�n al GameObject basado en giroscopio
            transform.rotation = initialRotation * (gyro.attitude * rotFix);
        }
    }
}