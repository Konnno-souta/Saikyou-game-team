using UnityEngine;
using System.Collections;

public class RankingUIController : MonoBehaviour
{
    public RectTransform rankingPanel;

    public float showY = 0f;      // 表示位置
    public float hideY = 800f;    // 非表示位置（画面外）
    public float slideTime = 0.5f;

    private Coroutine slideCoroutine;

    // ランキング表示（タイトルのボタン用）
    public void ShowRanking()
    {
        StartSlide(showY);
    }

    // ランキング非表示（閉じるボタン用）
    public void HideRanking()
    {
        StartSlide(hideY);
    }

    void StartSlide(float targetY)
    {
        if (slideCoroutine != null)
        {
            StopCoroutine(slideCoroutine);
        }
        slideCoroutine = StartCoroutine(Slide(targetY));
    }

    IEnumerator Slide(float targetY)
    {
        float startY = rankingPanel.anchoredPosition.y;
        float elapsed = 0f;

        while (elapsed < slideTime)
        {
            elapsed += Time.deltaTime;
            float y = Mathf.Lerp(startY, targetY, elapsed / slideTime);
            rankingPanel.anchoredPosition =
                new Vector2(rankingPanel.anchoredPosition.x, y);
            yield return null;
        }

        rankingPanel.anchoredPosition =
            new Vector2(rankingPanel.anchoredPosition.x, targetY);
    }
}
