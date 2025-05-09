using Platformer;
using UnityEngine;
using UnityEngine.UI;

public class Checkpoint : MonoBehaviour
{
    private bool activated = false;
    public Text checkpointText;

    public float fadeDuration = 0.5f;
    public float displayDuration = 1.5f;

    private string checkpointID;

    private void Start()
    {
        checkpointID = gameObject.name;

        // Check if checkpoint was already activated in saved data
        if (PlayerPrefs.GetInt(checkpointID, 0) == 1)
        {
            Destroy(gameObject); // Already activated before, destroy it immediately
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!activated && (other.CompareTag("Player1") || other.CompareTag("Player2")))
        {
            activated = true;

            // Save checkpoint progress
            PlayerPrefs.SetInt(checkpointID, 1);
            PlayerPrefs.Save();

            GameManager.instance.SaveCheckpoint();
            Debug.Log("Checkpoint reached!");

            if (checkpointText != null)
            {
                StartCoroutine(FadeCheckpointText());
            }
        }
    }

    private System.Collections.IEnumerator FadeCheckpointText()
    {
        // Setup text
        checkpointText.text = "Checkpoint Saved!";
        checkpointText.enabled = true;

        Color color = checkpointText.color;
        color.a = 0;
        checkpointText.color = color;

        // Fade In
        float timer = 0f;
        while (timer <= fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, timer / fadeDuration);
            checkpointText.color = color;
            yield return null;
        }

        // Stay fully visible
        yield return new WaitForSeconds(displayDuration);

        // Fade Out
        timer = 0f;
        while (timer <= fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, timer / fadeDuration);
            checkpointText.color = color;
            yield return null;
        }

        checkpointText.enabled = false;
    }
}
