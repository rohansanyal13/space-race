using UnityEngine;

namespace Platformer
{
    [RequireComponent(typeof(Camera))]
    public class CameraFollowPlayersWithZoom : MonoBehaviour
    {
        public Transform player1;
        public Transform player2;

        public float smoothSpeed = 0.125f;
        public Vector3 offset;

        // Optional level bounds
        public Vector2 minPosition;
        public Vector2 maxPosition;

        // Zoom control
        public float minZoom = 5f;
        public float maxZoom = 10f;
        public float zoomLimiter = 10f;
        public float zoomSpeed = 5f;

        private Camera cam;

        private void Start()
        {
            cam = GetComponent<Camera>();

            if (GameManager.instance != null)
            {
                player1 = GameManager.instance.player1GameObject?.transform;
                player2 = GameManager.instance.player2GameObject?.transform;
            }
        }

        private void LateUpdate()
        {
            if (player1 == null || player2 == null) return;

            // Calculate midpoint between players
            Vector3 middlePoint = (player1.position + player2.position) / 2f;
            Vector3 desiredPosition = middlePoint + offset;

            // Clamp position to level bounds
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minPosition.x, maxPosition.x);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minPosition.y, maxPosition.y);
            desiredPosition.z = -10f; // Keep camera in front

            // Smooth follow
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // Dynamic zoom: zoom out as players separate
            float distance = Vector3.Distance(player1.position, player2.position);
            float targetZoom = Mathf.Clamp(minZoom + distance / zoomLimiter, minZoom, maxZoom);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
        }
    }
}
