using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionTrigger : MonoBehaviour
{
    public string nextSceneName;    // <- set this in Inspector
    public Animator fadeAnimator;   // <- set this in Inspector
    public float transitionDelay = 1.5f;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasTriggered && (other.CompareTag("Player1") || other.CompareTag("Player2")))
        {
            hasTriggered = true;
            if (fadeAnimator != null)
            {
                fadeAnimator.SetTrigger("FadeOut");
            }
            Invoke(nameof(LoadNextScene), transitionDelay);
        }
    }


    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
