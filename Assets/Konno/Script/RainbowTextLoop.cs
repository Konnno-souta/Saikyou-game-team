using UnityEngine;
using TMPro;
using System.Collections;

public class RainbowTextLoop : MonoBehaviour
{
    public float colorSpeed = 2f;   // 虹の変化スピード
    public float charDelay = 0.05f; // 文字ごとのずれ

    TMP_Text text;
    TMP_TextInfo textInfo;

    void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    void Start()
    {
        StartCoroutine(RainbowLoop());
    }

    IEnumerator RainbowLoop()
    {
        while (true) //  永遠ループ
        {
            text.ForceMeshUpdate();
            textInfo = text.textInfo;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                if (!textInfo.characterInfo[i].isVisible)
                    continue;

                int matIndex = textInfo.characterInfo[i].materialReferenceIndex;
                int vertIndex = textInfo.characterInfo[i].vertexIndex;

                Vector3[] vertices = textInfo.meshInfo[matIndex].vertices;
                Color32[] colors = textInfo.meshInfo[matIndex].colors32;

                float hue = (Time.time * colorSpeed + i * charDelay) % 1f;
                Color rainbow = Color.HSVToRGB(hue, 1f, 1f);

                colors[vertIndex + 0] = rainbow;
                colors[vertIndex + 1] = rainbow;
                colors[vertIndex + 2] = rainbow;
                colors[vertIndex + 3] = rainbow;
            }

            // メッシュ反映
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                textInfo.meshInfo[i].mesh.colors32 = textInfo.meshInfo[i].colors32;
                text.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }

            yield return null;
        }
    }
}

