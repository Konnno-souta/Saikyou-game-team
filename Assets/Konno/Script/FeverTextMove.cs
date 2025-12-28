using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class FeverTextMove : MonoBehaviour
{
    RectTransform rect;
    Graphic graphic;   // Text / TMP 共通
    Vector3 baseScale;

    public float moveTime = 3.0f;
    public float stopTime = 0.5f;

    Vector2 leftPos;
    Vector2 centerPos;
    Vector2 rightPos;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        graphic = GetComponent<Graphic>(); // Text or TMP
        baseScale = rect.localScale;

        float canvasWidth = rect.root.GetComponent<RectTransform>().sizeDelta.x;

        leftPos = new Vector2(-canvasWidth / 2 - rect.sizeDelta.x, 0);
        centerPos = Vector2.zero;
        rightPos = new Vector2(canvasWidth / 2 + rect.sizeDelta.x, 0);

        rect.anchoredPosition = leftPos;

        StartCoroutine(FeverSequence());
    }

    IEnumerator FeverSequence()
    {
        // ① 左 → 中央
        yield return StartCoroutine(Move(leftPos, centerPos, moveTime));

        // ② 中央停止中：拡大＋点滅
        yield return StartCoroutine(ScaleBlink(stopTime));

        // ③ 中央 → 右
        yield return StartCoroutine(Move(centerPos, rightPos, moveTime));
        Destroy(gameObject);
    }

    IEnumerator Move(Vector2 start, Vector2 end, float time)
    {
        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;
            float rate = Mathf.Clamp01(t / time);

            // Ease
            float ease = Mathf.SmoothStep(0f, 1f, rate);

            rect.anchoredPosition = Vector2.Lerp(start, end, ease);
            yield return null;
        }

        rect.anchoredPosition = end;
    }

    IEnumerator ScaleBlink(float duration)
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float rate = t / duration;

            // 拡大（ピョンと）
            float scale = Mathf.Lerp(1.0f, 1.2f, Mathf.Sin(rate * Mathf.PI));
            rect.localScale = baseScale * scale;

            // 点滅（アルファ）
            float alpha = Mathf.Lerp(0.3f, 1.0f, Mathf.PingPong(rate * 4f, 1f));
            Color c = graphic.color;
            c.a = alpha;
            graphic.color = c;

            yield return null;
        }

        // 後始末
        rect.localScale = baseScale;
        Color endColor = graphic.color;
        endColor.a = 1f;
        graphic.color = endColor;
    }
}





