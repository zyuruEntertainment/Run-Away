
using UnityEngine;

public class ObjectFloating : MonoBehaviour
{
    [Header("Gerakan Ombang-Ambing")]
    public float amplitude = 0.3f;
    public float frequency = 1f;

    [Header("Rotasi Perlahan")]
    public float rotationAmount = 3f;

    private Vector3 startPosition;
    private Quaternion startRotation;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void Update()
    {
        // Gerakan naik turun perlahan
        float newY = startPosition.y +
                     Mathf.Sin(Time.time * frequency) * amplitude;

        transform.position = new Vector3(
            startPosition.x,
            newY,
            startPosition.z
        );

        // Rotasi kecil agar terlihat melayang
        float rotationX = Mathf.Sin(Time.time * frequency * 0.7f)
                          * rotationAmount;

        float rotationZ = Mathf.Cos(Time.time * frequency * 0.5f)
                          * rotationAmount;

        transform.rotation = startRotation *
            Quaternion.Euler(rotationX, 0f, rotationZ);
    }
}
