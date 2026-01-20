using UnityEngine;
using TMPro;
using System.Collections;

public class ScorePopup : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // TextMeshProUGUI をセット
    public float displayTime = 1.0f;  // 表示時間

    public void ShowScore(int score)
    {
        scoreText.text = "+" + score.ToString();
        scoreText.gameObject.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(HideAfterTime());
    }

    IEnumerator HideAfterTime()
    {
        yield return new WaitForSeconds(displayTime);
        scoreText.gameObject.SetActive(false);
    }
}
