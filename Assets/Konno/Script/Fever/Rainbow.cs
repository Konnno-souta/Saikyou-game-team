using UnityEngine;
using TMPro;
using System.Collections;

public class Rainbow : MonoBehaviour
{
    public float colorSpeed = 2f;
    public float charDelay = 0.05f;

    TMP_Text text;
    TMP_TextInfo textInfo;
    Coroutine loop;

    void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    public void Play()
    {
        if (loop == null)
            loop = StartCoroutine(RainbowLoop());
    }

    public void Stop()
    {
        if (loop != null)
        {
            StopCoroutine(loop);
            loop = null;
            ResetColor();
        }
    }

    IEnumerator RainbowLoop()
    {
        while (true)
        {
            text.ForceMeshUpdate();
            textInfo = text.textInfo;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                if (!textInfo.characterInfo[i].isVisible)
                    continue;

                int matIndex = textInfo.characterInfo[i].materialReferenceIndex;
                int vertIndex = textInfo.characterInfo[i].vertexIndex;

                Color32[] colors = textInfo.meshInfo[matIndex].colors32;

                float hue = (Time.time * colorSpeed + i * charDelay) % 1f;
                Color rainbow = Color.HSVToRGB(hue, 1f, 1f);

                colors[vertIndex + 0] = rainbow;
                colors[vertIndex + 1] = rainbow;
                colors[vertIndex + 2] = rainbow;
                colors[vertIndex + 3] = rainbow;
            }

            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                textInfo.meshInfo[i].mesh.colors32 = textInfo.meshInfo[i].colors32;
                text.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }

            yield return null;
        }
    }

    void ResetColor()
    {
        text.color = Color.white;
    }
}
