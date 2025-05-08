using Platformer;
using UnityEngine;

public class Fuel : MonoBehaviour
{
    public string fuelID; // Unique ID for saving
    public int amount = 1; // Fuel amount collected

    private void Awake()
    {
        // Auto-assign a unique ID if not set manually
        if (string.IsNullOrEmpty(fuelID))
        {
            fuelID = gameObject.scene.name + "_" + transform.position.ToString();
        }
    }

    private void Start()
    {
        // Destroy if already collected
        if (PlayerPrefs.GetString("CollectedFuel", "").Contains(fuelID))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            GameManager.instance.CollectFuel(fuelID, amount);
            Destroy(gameObject);
        }
    }
}
