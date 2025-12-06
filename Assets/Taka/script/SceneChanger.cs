using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneChanger_NoCoroutine : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;

    private bool isFadingOut = false;
    private bool isFadingIn = true;
    private float timer = 0f;
    private string nextScene;

    void Start()
    {
        // 開始時はフェードインするため、Image は黒で始めておくこと
        fadeImage.color = new Color(0, 0, 0, 1f);
    }

    void Update()
    {
        if (isFadingIn)
        {
            timer += Time.deltaTime;
            float alpha = 1f - (timer / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);

            if (timer >= fadeDuration)
            {
                isFadingIn = false;
                timer = 0f;
            }
        }
        else if (isFadingOut)
        {
            timer += Time.deltaTime;
            float alpha = (timer / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);

            if (timer >= fadeDuration)
            {
                SceneManager.LoadScene(nextScene);
            }
        }
    }

    public void SceneChanger(string sceneName)
    {
        nextScene = sceneName;
        isFadingOut = true;
        timer = 0f;
    }
}
