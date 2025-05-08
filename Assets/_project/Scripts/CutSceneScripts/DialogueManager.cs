using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public Text dialogueText;
    public GameObject dialogueBox;
    public Image fadeImage; // Fullscreen black image
    public Text continueText; // "Press Space to Continue"
    public float continueTextFadeDuration = 0.5f; // Duration of fade in for continue text

    public float typingSpeed = 0.02f;

    private Queue<string> sentences;
    private bool isTyping = false;
    private CameraShake cameraShake;
    private bool isFirstSentence = true;

    private void Start()
    {
        sentences = new Queue<string>();
        cameraShake = Camera.main.GetComponent<CameraShake>();
        continueText.gameObject.SetActive(false);

        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        fadeImage.gameObject.SetActive(true);
        Color color = fadeImage.color;
        color.a = 1f;
        fadeImage.color = color;

        // Start camera shake during fade-in
        StartCoroutine(cameraShake.Shake(1.0f, 0.2f));

        while (color.a > 0)
        {
            color.a -= Time.deltaTime;
            fadeImage.color = color;
            yield return null;
        }

        fadeImage.gameObject.SetActive(false);
        StartDialogue();
    }

    void StartDialogue()
    {
        sentences.Clear();

        sentences.Enqueue("Explorer 1: Well... that wasn't exactly a smooth landing.");
        sentences.Enqueue("Explorer 2: Could've been worse. At least we still have gravity... I think.");
        sentences.Enqueue("Explorer 1: Where the hell are we?");
        sentences.Enqueue("Explorer 2: Looks like... an old underground energy research site. Abandoned. Mostly.");
        sentences.Enqueue("Explorer 1: Mostly?");
        sentences.Enqueue("Explorer 2: Long story short: if we don't find power cores and reboot the reactor, we're not getting off this rock.");
        sentences.Enqueue("Explorer 1: Alright. Scavenger hunt for survival. Just another Tuesday.");
        sentences.Enqueue("Explorer 2: We stick together. These places are full of old security systems - some of it's probably still active.");
        sentences.Enqueue("Explorer 1: Hazards. Traps. Death. My favorite kind of tour guide.");
        sentences.Enqueue("Explorer 2: Remember: some things might hit you and not me. Or the other way around. Or both?");
        sentences.Enqueue("Explorer 1: Trust. Got it. You fall, I catch. I fall, you figure something out.");
        sentences.Enqueue("Explorer 2: We both make it back. That's the deal.");
        sentences.Enqueue("Explorer 1: C'mon. Let's go find us some cores... before this place finishes falling apart.");

        DisplayNextSentence();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isTyping)
        {
            DisplayNextSentence();
        }
    }

    public void DisplayNextSentence()
    {
        // Hide the continue text immediately
        continueText.gameObject.SetActive(false);

        // Reset alpha to make it invisible
        Color textColor = continueText.color;
        textColor.a = 0f;
        continueText.color = textColor;

        if (sentences.Count == 0)
        {
            StartCoroutine(FadeOut());
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));

        // Only shake for first sentence 
        if (isFirstSentence)
        {
            StartCoroutine(cameraShake.Shake(0.5f, 0.2f));
            isFirstSentence = false;
        }
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        // Fade in the continue text
        StartCoroutine(FadeContinueText());
    }

    IEnumerator FadeContinueText()
    {
        continueText.gameObject.SetActive(true);
        Color textColor = continueText.color;
        textColor.a = 0f;
        continueText.color = textColor;

        float elapsedTime = 0f;

        while (elapsedTime < continueTextFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            textColor.a = Mathf.Clamp01(elapsedTime / continueTextFadeDuration);
            continueText.color = textColor;
            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        fadeImage.gameObject.SetActive(true);
        Color color = fadeImage.color;
        color.a = 0f;
        fadeImage.color = color;

        while (color.a < 1)
        {
            color.a += Time.deltaTime;
            fadeImage.color = color;
            yield return null;
        }

        Debug.Log("Cutscene Ended - Loading StartScene");
        UnityEngine.SceneManagement.SceneManager.LoadScene("StartScene");
    }
}