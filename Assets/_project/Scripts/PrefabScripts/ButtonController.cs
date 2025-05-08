using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [Header("Button Settings")]
    [SerializeField] private Sprite buttonUnpressedSprite;
    [SerializeField] private Sprite buttonPressedSprite;
    [SerializeField] private float buttonActivationDuration = 0.2f;
    [SerializeField] private float pulseMagnitude = 0.1f;

    [Header("Who Can Use the Button")]
    [SerializeField] private bool onlyPlayer1CanUse = false;
    [SerializeField] private bool onlyPlayer2CanUse = false;

    [Header("Door Settings")]
    [SerializeField] private SimpleButtonDoorController doorController;

    private SpriteRenderer spriteRenderer;
    private Vector3 originalScale;
    private int playersOnButton = 0;
    private bool isPressed = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;

        if (buttonUnpressedSprite != null)
            spriteRenderer.sprite = buttonUnpressedSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (CanActivateButton(other))
        {
            playersOnButton++;
            if (!isPressed)
            {
                SetButtonState(true);
                doorController?.ButtonPressed();
                OnButtonPressed();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (CanActivateButton(other))
        {
            playersOnButton--;
            if (playersOnButton <= 0)
            {
                SetButtonState(false);
                doorController?.ButtonReleased();
                OnButtonReleased();
                playersOnButton = 0;
            }
        }
    }

    private bool CanActivateButton(Component other)
    {
        if (onlyPlayer1CanUse) return other.CompareTag("Player1");
        if (onlyPlayer2CanUse) return other.CompareTag("Player2");
        return other.CompareTag("Player1") || other.CompareTag("Player2");
    }

    private void SetButtonState(bool pressed)
    {
        isPressed = pressed;
        StartCoroutine(TransitionButtonSprite(pressed)); // <-- DELAY here
    }


    private System.Collections.IEnumerator TransitionButtonSprite(bool pressed)
    {
        yield return PulseEffect();
        spriteRenderer.sprite = pressed ? buttonPressedSprite : buttonUnpressedSprite;
    }

    private System.Collections.IEnumerator PulseEffect()
    {
        float elapsedTime = 0;
        float halfDuration = buttonActivationDuration * 0.5f;
        Vector3 targetScale = originalScale * (1 + pulseMagnitude);

        // Scale up
        while (elapsedTime < halfDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / halfDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Scale down
        elapsedTime = 0;
        while (elapsedTime < halfDuration)
        {
            transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / halfDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
    }

    // These can be overridden by subclasses
    protected virtual void OnButtonPressed() { }
    protected virtual void OnButtonReleased() { }
}
