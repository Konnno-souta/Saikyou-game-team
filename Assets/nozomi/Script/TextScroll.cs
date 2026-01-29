using UnityEngine;

public class TextScroll : MonoBehaviour
{
    public float speed = 200f;
    [SerializeField] float stopTime = 1.5f; // íÜâõÇ≈é~Ç‹ÇÈïbêî

    RectTransform rectTransform;
    float canvasWidth;
    float textWidth;

    bool isStopping = false;
    bool isFinished = false;
    float timer = 0f;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        Canvas canvas = GetComponentInParent<Canvas>();
        canvasWidth = canvas.GetComponent<RectTransform>().sizeDelta.x;

        textWidth = rectTransform.sizeDelta.x;

        // ç∂âÊñ äOÇ©ÇÁÉXÉ^Å[Ég
        rectTransform.anchoredPosition =
            new Vector2(-canvasWidth / 2 - textWidth, rectTransform.anchoredPosition.y);
    }

    void Update()
    {
        Debug.Log(timer);
        // íÜâõÇ≈í‚é~íÜ
        if (isStopping)
        {
            timer += Time.deltaTime;
            if (timer >= stopTime)
            {
                isStopping = false; // çƒÇ—ìÆÇ©Ç∑
                timer = 0;
            }
        }
        else
        {
            // âEÇ÷à⁄ìÆ
            rectTransform.anchoredPosition += Vector2.right * speed * Time.deltaTime;
        }


        // íÜâõÇ…óàÇΩÇÁé~ÇﬂÇÈ
        if (!isFinished && rectTransform.anchoredPosition.x >= 0f)
        {
            isStopping = true;
            isFinished = true;
            rectTransform.anchoredPosition = new Vector2(0f, rectTransform.anchoredPosition.y);
        }
    }
}
