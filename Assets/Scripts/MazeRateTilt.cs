using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MazeRateTilt : MonoBehaviour
{
    public float degreesPerSecond = 60f;
    public float deadzoneDegrees = 3f;
    public float smoothing = 8f;

    public bool simulateInEditor = true;
    public float simInputSmoothing = 10f;

    public bool autoCalibrateOnStart = true;

    private Rigidbody rb;

    private Gyroscope gyro;
    private bool gyroEnabled;

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
            CalibrateCenter();
    }

    public void CalibrateCenter()
    {
        if (!gyroEnabled || gyro == null)
        {
            reference = Quaternion.identity;
            calibrated = true;
            return;
        }

        reference = GyroToUnity(gyro.attitude);
        calibrated = true;
    }

    private void FixedUpdate()
    {
        if (!calibrated) return;

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

            targetPitchDir = Mathf.Abs(pitchDeg) > deadzoneDegrees ? Mathf.Sign(pitchDeg) : 0f;
            targetRollDir = Mathf.Abs(rollDeg) > deadzoneDegrees ? Mathf.Sign(rollDeg) : 0f;

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