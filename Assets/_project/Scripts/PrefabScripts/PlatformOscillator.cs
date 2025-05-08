using UnityEngine;

public class PlatformOscillator : MonoBehaviour
{
    public enum MovementDirection { Horizontal, Vertical }
    public enum StartDirection { Direction1, Direction2 } // Right/Up vs. Left/Down

    [Header("Movement Settings")]
    public MovementDirection movementDirection = MovementDirection.Horizontal;
    public StartDirection startDirection = StartDirection.Direction1;

    public float moveAmount = 0.1f;  // How far it moves from the center
    public float moveSpeed = 2.0f;   // How fast it moves

    private Vector3 originalPosition;
    private float phaseOffset;

    void Start()
    {
        originalPosition = transform.position;
        // Flip the sine wave phase if starting in Direction2
        phaseOffset = (startDirection == StartDirection.Direction2) ? Mathf.PI : 0f;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * moveSpeed + phaseOffset) * moveAmount;

        Vector3 newPosition = originalPosition;

        if (movementDirection == MovementDirection.Horizontal)
        {
            newPosition.x += offset;
        }
        else if (movementDirection == MovementDirection.Vertical)
        {
            newPosition.y += offset;
        }

        transform.position = newPosition;
    }
}
