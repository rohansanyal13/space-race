using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndSequenceTrigger : MonoBehaviour
{
    public GameObject screenTextCanvas;  // Add this at the top

    public GameObject endCanvas;
    public Text endMessageText;
    public Text statsText;

    private bool player1Triggered = false;
    private bool player2Triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player1") && !player1Triggered)
        {
            player1Triggered = true;
            StartCoroutine(FadeOutPlayer(other.gameObject));
        }

        if (other.CompareTag("Player2") && !player2Triggered)
        {
            player2Triggered = true;
            StartCoroutine(FadeOutPlayer(other.gameObject));
        }

        if (player1Triggered && player2Triggered)
        {
            ShowEndScreen();
        }
    }

    private IEnumerator FadeOutPlayer(GameObject player)
    {
        float duration = 1.5f;
        float elapsed = 0f;

        Vector3 startScale = player.transform.localScale;
        Vector3 startPos = player.transform.position;
        Vector3 endPos = startPos + new Vector3(0, 1f, 0); // Move up by 1 unit

        float rotationSpeed = 720f; // degrees per second

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            player.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            player.transform.position = Vector3.Lerp(startPos, endPos, t); // Move upward
            player.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime); // Spin

            elapsed += Time.deltaTime;
            yield return null;
        }

        player.SetActive(false);
    }

    private void ShowEndScreen()
    {
        if (screenTextCanvas != null)
            screenTextCanvas.SetActive(false);

        endCanvas.SetActive(true);
        if (endMessageText != null)
            endMessageText.text = "THE END";

        if (statsText != null)
        {
            statsText.text =
                $"Coins Collected: {Platformer.GameManager.instance.coinsCounter}\n" +
                $"Energy Collected: {Platformer.GameManager.instance.fuelCounter}\n\n" +
                $"Explorer 1 Deaths: {Platformer.GameManager.player1DeathCounter}\n" +
                $"Explorer 2 Deaths: {Platformer.GameManager.player2DeathCounter}";
        }
    }
}
