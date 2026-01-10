using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RankingUIController : MonoBehaviour
{
    public RectTransform rankingPanel;

    public float showY = 0f;      // 表示位置
    public float hideY = 800f;    // 非表示位置
    public float slideTime = 0.5f;

    public Button firstSelectButton; // ★ 表示後に選択させたいボタン

    private Coroutine slideCoroutine;

    // ランキング表示（タイトルのボタン用）
    public void ShowRanking()
    {
        StartSlide(showY, true);
    }

    // ランキング非表示（閉じるボタン用）
    public void HideRanking()
    {
        StartSlide(hideY, false);
    }

    void StartSlide(float targetY, bool selectAfter)
    {
        if (slideCoroutine != null)
        {
            StopCoroutine(slideCoroutine);
        }
        slideCoroutine = StartCoroutine(Slide(targetY, selectAfter));
    }

    IEnumerator Slide(float targetY, bool selectAfter)
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

        // ★ 表示し終わったらボタンを選択
        if (selectAfter && firstSelectButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectButton.gameObject);
        }
    }
}
