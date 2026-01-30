using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonBlink : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    Image image;
    Coroutine blink;

    Color normalColor;
    Color darkColor;

    void Awake()
    {
        image = GetComponent<Image>();
        normalColor = image.color;
        darkColor = normalColor * 0.5f; // à√Ç≠Ç∑ÇÈÅií≤êÆâ¬Åj
        darkColor.a = normalColor.a;    // ìßñæìxÇÕÇªÇÃÇ‹Ç‹
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (blink == null)
            blink = StartCoroutine(Blink());
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (blink != null)
        {
            StopCoroutine(blink);
            blink = null;
            image.color = normalColor; // å≥Ç…ñﬂÇ∑
        }
    }

    IEnumerator Blink()
    {
        while (true)
        {
            image.color = darkColor;
            yield return new WaitForSeconds(0.3f);

            image.color = normalColor;
            yield return new WaitForSeconds(0.3f);
        }
    }
}

