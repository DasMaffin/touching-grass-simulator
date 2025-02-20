using UnityEngine;

public class VendorIconController : MonoBehaviour
{
    [Header("Bobbing Settings")]
    public float bobbingAmplitude = 0.5f; // How far it moves up and down
    public float bobbingSpeed = 2f; // Speed of the bobbing motion

    [Header("Rotation Settings")]
    public float rotationSpeed = 50f; // Speed of the rotation (degrees per second)

    private Vector3 initialPosition; // To store the original position of the object

    void Start()
    {
        // Record the initial position of the object
        initialPosition = transform.position;
    }

    void Update()
    {
        // Bobbing motion: Add a vertical offset based on a sine wave
        float bobbingOffset = Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmplitude;
        transform.position = initialPosition + new Vector3(0, bobbingOffset, 0);

        // Rotation motion: Rotate around the Y-axis
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }
}
