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

    [Header("Rainbow")]
    public float rainbowSpeed = 1.5f; // 虹の回転スピード

    void UpdateRainbow(float alpha = 1f)
    {
        float hue = Mathf.Repeat(Time.time * rainbowSpeed, 1f);
        Color c = Color.HSVToRGB(hue, 1f, 1f);
        c.a = alpha;
        graphic.color = c;
    }

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        graphic = GetComponent<Graphic>();
        baseScale = rect.localScale;

        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);

        RectTransform canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        float canvasWidth = canvasRect.sizeDelta.x;

        leftPos = new Vector2(-canvasWidth / 2 - rect.sizeDelta.x, 0);
        centerPos = Vector2.zero;
        rightPos = new Vector2(canvasWidth / 2 + rect.sizeDelta.x, 0);

        rect.anchoredPosition = leftPos;

        // 出現した瞬間から虹色
        UpdateRainbow(1f);
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

            // 移動中も虹色更新
            UpdateRainbow(1f);

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

            float alpha = Mathf.Lerp(0.3f, 1f, Mathf.PingPong(rate * 4f, 1f));
            UpdateRainbow(alpha);

            yield return null;
        }

        rect.localScale = baseScale;
        UpdateRainbow(1f);
    }

}
