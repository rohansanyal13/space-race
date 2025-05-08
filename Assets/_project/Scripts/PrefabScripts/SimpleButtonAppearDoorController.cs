using UnityEngine;
using System.Collections;

public class SimpleButtonAppearDoorController : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private SpriteRenderer doorRenderer;
    [SerializeField] private Collider2D doorCollider;
    [SerializeField] private float doorFadeDuration = 1.0f;

    private int pressedButtonsCount = 0;
    private bool doorVisible = false;

    public void ButtonPressed()
    {
        pressedButtonsCount++;
        UpdateDoorState();
    }

    public void ButtonReleased()
    {
        pressedButtonsCount--;
        UpdateDoorState();
    }

    private void UpdateDoorState()
    {
        if (pressedButtonsCount > 0 && !doorVisible)
        {
            ShowDoor();
        }
        else if (pressedButtonsCount == 0 && doorVisible)
        {
            HideDoor();
        }
    }

    private void ShowDoor()
    {
        doorVisible = true;
        if (doorRenderer != null) StartCoroutine(FadeDoor(true)); // fade in
        if (doorCollider != null) doorCollider.enabled = true;
    }

    private void HideDoor()
    {
        doorVisible = false;
        if (doorRenderer != null) StartCoroutine(FadeDoor(false)); // fade out
        if (doorCollider != null) doorCollider.enabled = false;
    }

    private IEnumerator FadeDoor(bool fadeIn)
    {
        float elapsedTime = 0;
        Color originalColor = doorRenderer.color;
        float startAlpha = fadeIn ? 0 : 1;
        float endAlpha = fadeIn ? 1 : 0;

        while (elapsedTime < doorFadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / doorFadeDuration);
            doorRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        doorRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, endAlpha);
    }
}
