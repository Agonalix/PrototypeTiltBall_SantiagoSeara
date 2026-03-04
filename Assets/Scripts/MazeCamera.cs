using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCamera : MonoBehaviour
{
    [Header("Refs")]
    public Transform center; // BallCenter
    public Transform ball;   // Bolita (opcional)

    [Header("Position")]
    public Vector3 offset = new Vector3(0f, 7f, -10f);
    public float posSmooth = 8f;

    [Header("Look")]
    [Range(0f, 1f)] public float lookAtBallWeight = 0.25f; // 0=centro, 1=bola
    public float lookSmooth = 10f;

    void LateUpdate()
    {
        if (center == null) return;

        // posición fija respecto al centro (no sigue a la bola)
        Vector3 desiredPos = center.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, posSmooth * Time.deltaTime);

        // mira al centro (o un mix con la bola)
        Vector3 lookPoint = center.position;
        if (ball != null)
            lookPoint = Vector3.Lerp(center.position, ball.position, lookAtBallWeight);

        Quaternion desiredRot = Quaternion.LookRotation(lookPoint - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, lookSmooth * Time.deltaTime);
    }
}