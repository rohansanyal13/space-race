using Platformer;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public string coinID; // Unique ID for saving

    private void Awake()
    {
        // Auto-assign a unique ID if not set manually
        if (string.IsNullOrEmpty(coinID))
        {
            coinID = gameObject.scene.name + "_" + transform.position.ToString();
        }
    }

    private void Start()
    {
        // Destroy if already collected
        if (PlayerPrefs.GetString("CollectedCoins", "").Contains(coinID))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            GameManager.instance.CollectCoin(coinID);
            Destroy(gameObject);
        }
    }
}

