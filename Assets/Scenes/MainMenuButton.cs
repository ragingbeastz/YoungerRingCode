using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;


public class MainMenuButton : MonoBehaviour
{  
    public Image transitionImage;
    public AudioSource MainTheme;
    public AudioClip StartGameSound;
    private float transitionDuration = 2f;
    private AudioSource audioSource;


    public void LoadGameScene()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        MainTheme.Stop();
        audioSource.PlayOneShot(StartGameSound);
        transitionImage.transform.SetAsLastSibling();
        StartCoroutine(FadeAndLoadScene());
    }

    private IEnumerator FadeAndLoadScene()
    {
        float elapsedTime = 0f;
        Color color = transitionImage.color;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / transitionDuration);
            transitionImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        SceneManager.LoadScene("YoungerRing");
    }


}
