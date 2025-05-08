using UnityEngine;

public class Lever : MonoBehaviour
{
    public GameObject spikes; // Assign the spikes GameObject in the inspector
    private bool isActivated = false; // Track the lever state

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure the player is the one triggering it
        {
            FlipLever();
        }
    }

    void FlipLever()
    {
        isActivated = !isActivated; // Toggle lever state
        spikes.SetActive(!isActivated); // Disable spikes when lever is activated
    }
}
