using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FeverTextMove : MonoBehaviour
{
    RectTransform rect;
    Graphic graphic;
    Vector3 baseScale;

    [Header("Time")]
    public float moveTime = 3.0f;
    public float stopTime = 0.5f;

    Vector2 leftPos;
    Vector2 centerPos;
    Vector2 rightPos;

    [Header("Auto Play")]
    public bool playOnStart = true;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        graphic = GetComponent<Graphic>();

        baseScale = rect.localScale;

        // Anchorを中央に強制（ズレ防止）
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);

        RectTransform canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        float canvasWidth = canvasRect.sizeDelta.x;

        leftPos = new Vector2(-canvasWidth / 2 - rect.sizeDelta.x, 0);
        centerPos = Vector2.zero;
        rightPos = new Vector2(canvasWidth / 2 + rect.sizeDelta.x, 0);

        rect.anchoredPosition = leftPos;
    }

    void Start()
    {
        if (playOnStart)
            Play();
    }

    // 外部から呼べる
    public void Play()
    {
        StartCoroutine(FeverSequence());
    }

    IEnumerator FeverSequence()
    {
        yield return Move(leftPos, centerPos, moveTime);
        yield return ScaleBlink(stopTime);
        yield return Move(centerPos, rightPos, moveTime);

        Destroy(gameObject);
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

    IEnumerator ScaleBlink(float duration)
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float rate = t / duration;

            float scale = Mathf.Lerp(1.0f, 1.2f, Mathf.Sin(rate * Mathf.PI));
            rect.localScale = baseScale * scale;

            Color c = graphic.color;
            c.a = Mathf.Lerp(0.3f, 1f, Mathf.PingPong(rate * 4f, 1f));
            graphic.color = c;

            yield return null;
        }

        rect.localScale = baseScale;
        Color endColor = graphic.color;
        endColor.a = 1f;
        graphic.color = endColor;
    }
}
