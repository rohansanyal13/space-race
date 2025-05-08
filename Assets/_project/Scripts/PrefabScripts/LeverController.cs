using UnityEngine;
using System.Collections;

public class LeverController : MonoBehaviour
{
    [Header("Lever Settings")]
    [SerializeField] private Sprite leverUpSprite;
    [SerializeField] private Sprite leverDownSprite;
    [SerializeField] private float leverActivationDuration = 0.5f;
    [SerializeField] private float pulseMagnitude = 0.2f; // How much to scale during pulse
    [SerializeField] private bool canToggle = true; // Whether lever can be switched back and forth
    [SerializeField] private bool startActivated = false; // Whether lever starts in the activated position

    [Header("Who Can Use the Lever")]
    [SerializeField] private bool onlyPlayer1CanUse = false;
    [SerializeField] private bool onlyPlayer2CanUse = false;

    [Header("Door Settings")]
    [SerializeField] private GameObject doorObject;
    [SerializeField] private float doorFadeDuration = 1.0f;

    private SpriteRenderer spriteRenderer;
    private bool isActivated = false;
    private Vector3 originalScale;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        // Store original scale for pulse effect
        originalScale = transform.localScale;

        // Initialize the lever state based on startActivated
        isActivated = startActivated;

        // Set initial sprite based on starting state
        if (isActivated && leverDownSprite != null)
        {
            spriteRenderer.sprite = leverDownSprite;
            // If starting activated, also set door state
            if (doorObject != null)
            {
                StartCoroutine(FadeDoorOut(true));
            }
        }
        else if (leverUpSprite != null)
        {
            spriteRenderer.sprite = leverUpSprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (CanActivateLever(other))
        {
            if (!isActivated)
            {
                ActivateLever(true);
            }
            else if (canToggle)
            {
                ActivateLever(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CanActivateLever(other))
        {
            if (!isActivated)
            {
                ActivateLever(true);
            }
            else if (canToggle)
            {
                ActivateLever(false);
            }
        }
    }

    private bool CanActivateLever(Component other)
    {
        if (onlyPlayer1CanUse)
            return other.CompareTag("Player1");

        if (onlyPlayer2CanUse)
            return other.CompareTag("Player2");

        // If neither specific restriction, allow both players
        return other.CompareTag("Player1") || other.CompareTag("Player2");
    }

    private void ActivateLever(bool activate)
    {
        isActivated = activate;

        // Start the lever sprite transition with a pulse effect
        StartCoroutine(TransitionLeverSprite(activate));

        // Handle the door
        if (activate)
        {
            StartCoroutine(FadeDoorOut(true));
        }
        else
        {
            StartCoroutine(FadeDoorOut(false));
        }
    }

    private IEnumerator TransitionLeverSprite(bool activate)
    {
        // Add a scale pulse effect
        StartCoroutine(PulseEffect());

        // Wait a moment
        yield return new WaitForSeconds(leverActivationDuration * 0.5f);

        // Switch to appropriate sprite
        if (activate && leverDownSprite != null)
        {
            spriteRenderer.sprite = leverDownSprite;
        }
        else if (!activate && leverUpSprite != null)
        {
            spriteRenderer.sprite = leverUpSprite;
        }
    }

    private IEnumerator PulseEffect()
    {
        float elapsedTime = 0;
        float halfDuration = leverActivationDuration * 0.25f;
        Vector3 targetScale = originalScale * (1 + pulseMagnitude);

        // Scale up
        while (elapsedTime < halfDuration)
        {
            float t = elapsedTime / halfDuration;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Scale back down
        elapsedTime = 0;
        while (elapsedTime < halfDuration)
        {
            float t = elapsedTime / halfDuration;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
    }

    private IEnumerator FadeDoorOut(bool fadeOut)
    {
        if (doorObject != null)
        {
            SpriteRenderer doorRenderer = doorObject.GetComponent<SpriteRenderer>();
            if (doorRenderer == null)
            {
                Renderer renderer3D = doorObject.GetComponent<Renderer>();
                if (renderer3D != null && renderer3D.material != null)
                {
                    StartCoroutine(Fade3DDoor(renderer3D, fadeOut));
                }
            }
            else
            {
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

            Collider2D doorCollider2D = doorObject.GetComponent<Collider2D>();
            if (doorCollider2D != null)
            {
                doorCollider2D.enabled = !fadeOut;
            }

            Collider doorCollider = doorObject.GetComponent<Collider>();
            if (doorCollider != null)
            {
                doorCollider.enabled = !fadeOut;
            }
        }
    }

    private IEnumerator Fade3DDoor(Renderer renderer3D, bool fadeOut)
    {
        float elapsedTime = 0;
        Color originalColor = renderer3D.material.color;

        Material doorMaterial = renderer3D.material;
        if (fadeOut && doorMaterial.GetFloat("_Mode") != 2)
        {
            doorMaterial.SetFloat("_Mode", 2);
            doorMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            doorMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            doorMaterial.SetInt("_ZWrite", 0);
            doorMaterial.DisableKeyword("_ALPHATEST_ON");
            doorMaterial.EnableKeyword("_ALPHABLEND_ON");
            doorMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            doorMaterial.renderQueue = 3000;
        }

        float startAlpha = fadeOut ? 1 : 0;
        float endAlpha = fadeOut ? 0 : 1;

        while (elapsedTime < doorFadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / doorFadeDuration);
            doorMaterial.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        doorMaterial.color = new Color(originalColor.r, originalColor.g, originalColor.b, endAlpha);
    }
}