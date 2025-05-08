using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorZone : MonoBehaviour
{
    public string playerTag; // "Player1" or "Player2"

    [Header("Door Sprites")]
    public SpriteRenderer doorRenderer;
    public Sprite doorClosedSprite;
    public Sprite doorOpenSprite;

    private static bool player1AtDoor = false;
    private static bool player2AtDoor = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            if (playerTag == "Player1")
                player1AtDoor = true;
            else if (playerTag == "Player2")
                player2AtDoor = true;

            SetDoorOpen(true);
            CheckBothPlayers();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            if (playerTag == "Player1")
                player1AtDoor = false;
            else if (playerTag == "Player2")
                player2AtDoor = false;

            SetDoorOpen(false);
        }
    }

    private void SetDoorOpen(bool isOpen)
    {
        if (doorRenderer != null && doorOpenSprite != null && doorClosedSprite != null)
        {
            doorRenderer.sprite = isOpen ? doorOpenSprite : doorClosedSprite;
        }
    }

    private void CheckBothPlayers()
    {
        if (player1AtDoor && player2AtDoor)
        {
            Debug.Log("Both players at their doors. Loading next scene...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
