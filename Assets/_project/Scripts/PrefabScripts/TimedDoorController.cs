using UnityEngine;
using System.Collections;

public class TimedDoorController : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private float doorFadeDuration = 1.0f;
    [SerializeField] private float startDelay = 2.0f;
    [SerializeField] private float delayBeforeFade = 2.0f;
    [SerializeField] private float timeDoorStaysInvisible = 5f;
    [SerializeField] private bool loop = true;

    private SpriteRenderer doorRenderer;
    private bool isFadedOut = false;

    private GameObject doorObject;

    void Start()
    {
        doorObject = gameObject; // Automatically use itself
        doorRenderer = doorObject.GetComponent<SpriteRenderer>();
        StartCoroutine(StartWithDelay());
    }


    private IEnumerator StartWithDelay()
    {
        yield return new WaitForSeconds(startDelay);
        StartCoroutine(DoorCycle());
    }

    private IEnumerator DoorCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(delayBeforeFade);
            yield return StartCoroutine(FadeDoor(true)); // Fade out

            isFadedOut = true;
            SetDoorCollider(false);

            yield return new WaitForSeconds(timeDoorStaysInvisible);
            yield return StartCoroutine(FadeDoor(false)); // Fade in

            isFadedOut = false;
            SetDoorCollider(true);

            if (!loop) break;
        }
    }

    private IEnumerator FadeDoor(bool fadeOut)
    {
        if (doorRenderer == null) yield break;

        float elapsedTime = 0;
        Color originalColor = doorRenderer.color;
        float startAlpha = fadeOut ? 1 : 0;
        float endAlpha = fadeOut ? 0 : 1;

        while (elapsedTime < doorFadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / doorFadeDuration);
            doorRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        doorRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, endAlpha);
    }

    private void SetDoorCollider(bool active)
    {
        Collider2D col2D = doorObject.GetComponent<Collider2D>();
        if (col2D != null)
            col2D.enabled = active;

        Collider col3D = doorObject.GetComponent<Collider>();
        if (col3D != null)
            col3D.enabled = active;
    }
}
