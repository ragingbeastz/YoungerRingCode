using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeIn : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 2f;

    private void Start()
    {
        // Ensure the image is fully opaque at the start
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 1f;
            fadeImage.color = color;
        }

        // Start the fade-in effect
        StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1.0f - (elapsedTime / fadeDuration));
            color.a = alpha;
            fadeImage.color = color;
            yield return null;
        }

        // Ensure the image is fully transparent at the end
        color.a = 0f;
        fadeImage.color = color;

        // Optionally, disable the fadeImage GameObject
        fadeImage.gameObject.SetActive(false);
    }
}
