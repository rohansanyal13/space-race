using UnityEngine;

public class TextShake : MonoBehaviour
{
    public float shakeAmount = 0.1f;    // How much the text shakes
    public float shakeSpeed = 2.0f;     // How fast the text shakes

    private Vector3 originalPosition;   // Store the original position

    void Start()
    {
        // Store the initial position when the game starts
        originalPosition = transform.position;
    }

    void Update()
    {
        // Calculate the offset using a sine wave for smooth up and down motion
        float yOffset = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;

        // Apply the offset to the original position
        transform.position = new Vector3(
            originalPosition.x,
            originalPosition.y + yOffset,
            originalPosition.z
        );
    }
}