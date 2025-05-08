using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    public float duration = 1.5f;
    private Image img;
    private Color startColor;

    void Start()
    {
        img = GetComponent<Image>();
        startColor = new Color(0, 0, 0, 0); // Transparent black
        img.color = startColor;
        StartCoroutine(FadeToBlack());
    }

    IEnumerator FadeToBlack()
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            img.color = Color.Lerp(startColor, Color.black, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        img.color = Color.black;
    }
}
