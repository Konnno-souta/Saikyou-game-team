using UnityEngine;
using System.Collections;
using TMPro;

public class FeverTextMove_TMP : MonoBehaviour
{
    RectTransform rect;
    TMP_Text tmp;
    Vector3 baseScale;

    public float moveTime = 3.0f;
    public float stopTime = 0.5f;
    public float rainbowSpeed = 2.0f;   // 虹色変化速度
    public float charOffset = 0.1f;      // 文字ごとの色ズレ

    Vector2 leftPos;
    Vector2 centerPos;
    Vector2 rightPos;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        tmp = GetComponent<TMP_Text>();
        baseScale = rect.localScale;

        float canvasWidth = rect.root.GetComponent<RectTransform>().sizeDelta.x;

        leftPos = new Vector2(-canvasWidth / 2 - rect.sizeDelta.x, 0);
        centerPos = Vector2.zero;
        rightPos = new Vector2(canvasWidth / 2 + rect.sizeDelta.x, 0);

        rect.anchoredPosition = leftPos;

        StartCoroutine(FeverSequence());
    }

    void Update()
    {
        UpdateRainbowPerCharacter();
    }

    IEnumerator FeverSequence()
    {
        // 左 → 中央
        yield return StartCoroutine(Move(leftPos, centerPos, moveTime));

        // 中央停止：拡大
        yield return StartCoroutine(ScalePunch(stopTime));

        // 中央 → 右
        yield return StartCoroutine(Move(centerPos, rightPos, moveTime));
    }

    IEnumerator Move(Vector2 start, Vector2 end, float time)
    {
        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;
            float rate = Mathf.Clamp01(t / time);
            float ease = Mathf.SmoothStep(0f, 1f, rate);

            rect.anchoredPosition = Vector2.Lerp(start, end, ease);
            yield return null;
        }

        rect.anchoredPosition = end;
    }

    IEnumerator ScalePunch(float duration)
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float rate = t / duration;

            float scale = Mathf.Lerp(1.0f, 1.2f, Mathf.Sin(rate * Mathf.PI));
            rect.localScale = baseScale * scale;

            yield return null;
        }

        rect.localScale = baseScale;
    }

    // 文字1文字ずつ虹色
    void UpdateRainbowPerCharacter()
    {
        tmp.ForceMeshUpdate();
        var textInfo = tmp.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int matIndex = charInfo.materialReferenceIndex;
            int vertIndex = charInfo.vertexIndex;

            // 文字ごとに位相をずらす
            float hue = Mathf.Repeat(Time.time * rainbowSpeed + i * charOffset, 1f);
            Color color = Color.HSVToRGB(hue, 1f, 1f);

            var colors = textInfo.meshInfo[matIndex].colors32;
            colors[vertIndex + 0] = color;
            colors[vertIndex + 1] = color;
            colors[vertIndex + 2] = color;
            colors[vertIndex + 3] = color;
        }

        // 反映
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            tmp.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}

