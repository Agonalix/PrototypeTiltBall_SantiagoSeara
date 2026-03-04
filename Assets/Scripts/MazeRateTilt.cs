using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MazeRateTilt : MonoBehaviour
{
    public float degreesPerSecond = 45f;
    public float deadzoneDegrees = 7f;
    public float smoothing = 10f;
    public float maxInputAngle = 15f;

    public bool simulateInEditor = true;
    public float simInputSmoothing = 10f;

    public bool autoCalibrateOnStart = true;

    private Rigidbody rb;

    private Gyroscope gyro;
    private bool gyroEnabled;
    private bool calibratedForPlay = false;

    private Quaternion reference;
    private bool calibrated;

    private float inputPitch;
    private float inputRoll;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void Start()
    {
        gyroEnabled = SystemInfo.supportsGyroscope;

        if (gyroEnabled)
        {
            gyro = Input.gyro;
            gyro.enabled = true;
        }
        else
        {
            Debug.LogWarning("No hay giroscopio. (Normal en Editor/PC).");
        }

        calibrated = false;

        if (autoCalibrateOnStart)
            CalibrateCenterForPlay();
    }

    public void CalibrateCenterForPlay()
    {
        if (!gyroEnabled || gyro == null)
        {
            reference = Quaternion.identity;
            calibrated = true;
            calibratedForPlay = true;
            return;
        }

        reference = GyroToUnity(gyro.attitude);
        calibrated = true;
        calibratedForPlay = true;
    }

    private void FixedUpdate()
    {
       if (!calibratedForPlay) return;

        float targetPitchDir = 0f;
        float targetRollDir = 0f;

        // Editor sim
        if ((!gyroEnabled || gyro == null) && simulateInEditor)
        {
            targetPitchDir =
                (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) ? 1f :
                (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) ? -1f : 0f;

            targetRollDir =
                (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) ? -1f :
                (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) ? 1f : 0f;

            inputPitch = Mathf.Lerp(inputPitch, targetPitchDir, simInputSmoothing * Time.fixedDeltaTime);
            inputRoll = Mathf.Lerp(inputRoll, targetRollDir, simInputSmoothing * Time.fixedDeltaTime);
        }
        else
        {
            if (!gyroEnabled || gyro == null) return;

            Quaternion current = GyroToUnity(gyro.attitude);
            Quaternion delta = Quaternion.Inverse(reference) * current;

            Vector3 euler = delta.eulerAngles;
            float pitchDeg = NormalizeAngle(euler.x);
            float rollDeg = NormalizeAngle(euler.z);

            float pitchNorm = Mathf.Clamp(pitchDeg / maxInputAngle, -1f, 1f);
            float rollNorm = Mathf.Clamp(rollDeg / maxInputAngle, -1f, 1f);

            // deadzone proporcional
            targetPitchDir = Mathf.Abs(pitchDeg) > deadzoneDegrees ? pitchNorm : 0f;
            targetRollDir = Mathf.Abs(rollDeg) > deadzoneDegrees ? rollNorm : 0f;

            inputPitch = Mathf.Lerp(inputPitch, targetPitchDir, smoothing * Time.fixedDeltaTime);
            inputRoll = Mathf.Lerp(inputRoll, targetRollDir, smoothing * Time.fixedDeltaTime);
        }

        float pitchStep = inputPitch * degreesPerSecond * Time.fixedDeltaTime;
        float rollStep = inputRoll * degreesPerSecond * Time.fixedDeltaTime;

        Quaternion stepRot = Quaternion.Euler(pitchStep, 0f, rollStep);
        rb.MoveRotation(rb.rotation * stepRot);
    }

    private static Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }

    private static float NormalizeAngle(float a)
    {
        if (a > 180f) a -= 360f;
        return a;
    }
}