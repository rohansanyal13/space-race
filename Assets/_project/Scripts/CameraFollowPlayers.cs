using UnityEngine;

namespace Platformer
{
    public class CameraFollowPlayers : MonoBehaviour
    {
        public Transform player1;
        public Transform player2;

        public float smoothSpeed = 0.125f;
        public Vector3 offset;

        // Optional: Level bounds
        public Vector2 minPosition;
        public Vector2 maxPosition;

        private void Start()
        {
            if (GameManager.instance != null)
            {
                player1 = GameManager.instance.player1GameObject?.transform;
                player2 = GameManager.instance.player2GameObject?.transform;
            }
        }

        private void LateUpdate()
        {
            if (player1 == null || player2 == null) return;

            Vector3 middlePoint = (player1.position + player2.position) / 2f;
            Vector3 desiredPosition = middlePoint + offset;

            // Optional: Clamp camera to boundaries
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minPosition.x, maxPosition.x);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minPosition.y, maxPosition.y);
            desiredPosition.z = -10f; // Make sure camera stays behind everything if using 2D default

            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        }
    }
}
