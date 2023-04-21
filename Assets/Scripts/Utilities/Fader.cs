using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneFader : MonoBehaviour {

    public Image fadeOutUIImage;
    public float fadeSpeed = 0.8f;
    public event Action OnFadeComplete;

    public enum FadeDirection {
        In, //Alpha = 1
        Out // Alpha = 0
    }


    void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator Fade(FadeDirection fadeDirection) {
        Debug.Log("Fading");

        float alpha = (fadeDirection == FadeDirection.Out) ? 1 : 0;
        float fadeEndValue = (fadeDirection == FadeDirection.Out) ? 0 : 1;
        if (fadeDirection == FadeDirection.Out) {
            while (alpha >= fadeEndValue) {
                alpha -= Time.deltaTime * fadeSpeed;
                SetColorImage(ref alpha, fadeDirection);
                yield return null;
            }
            fadeOutUIImage.enabled = false;
        } else {
            fadeOutUIImage.enabled = true;
            while (alpha <= fadeEndValue) {
                alpha += Time.deltaTime * fadeSpeed;
                SetColorImage(ref alpha, fadeDirection);
                yield return null;
            }
        }
    }
    public IEnumerator FadeAndLoadScene(string sceneToLoad) {
        Debug.Log("Fading and Loading Scene: " + sceneToLoad);
        float alpha = 1;
        fadeOutUIImage.color = new Color(fadeOutUIImage.color.r, fadeOutUIImage.color.g, fadeOutUIImage.color.b, alpha);
        yield return StartCoroutine(Fade(FadeDirection.In));
        Debug.Log("Finished Fading");

        SceneManager.LoadScene(sceneToLoad);
        yield return StartCoroutine(Fade(FadeDirection.Out));
        Debug.Log("Finished Fading");
        OnFadeComplete?.Invoke();
        Destroy(gameObject);
    }



    private void SetColorImage(ref float alpha, FadeDirection fadeDirection) {
        fadeOutUIImage.color = new Color(fadeOutUIImage.color.r, fadeOutUIImage.color.g, fadeOutUIImage.color.b, alpha);
        alpha += Time.deltaTime * (1.0f / fadeSpeed) * ((fadeDirection == FadeDirection.Out) ? -1 : 1);
    }
}

