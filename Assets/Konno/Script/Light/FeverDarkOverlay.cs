using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FeverDarkOverlay : MonoBehaviour
{
    public Image overlay;
    public float fadeTime = 0.3f;
    public float feverAlpha = 0.6f;

    Coroutine current;

    public void StartFever()
    {
        if (current != null) StopCoroutine(current);
        current = StartCoroutine(FadeTo(feverAlpha));
    }

    public void EndFever()
    {
        if (current != null) StopCoroutine(current);
        current = StartCoroutine(FadeTo(0f));
    }

    IEnumerator FadeTo(float target)
    {
        float start = overlay.color.a;
        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(start, target, t / fadeTime);
            overlay.color = new Color(0, 0, 0, a);
            yield return null;
        }
    }
}
