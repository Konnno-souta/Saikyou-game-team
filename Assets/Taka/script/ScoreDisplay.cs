using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // Inspectorでセット
    private float displayTime = 2f;
    private float timer = 0f;
    private bool isDisplaying = false;

    void Update()
    {
        if (isDisplaying)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                scoreText.text = "";
                isDisplaying = false;
            }
        }
    }

    // スコア表示用
    public void ShowScore(int score)
    {
        if (score > 0)
            scoreText.text = "+" + score;
        else
            scoreText.text = score.ToString();
        timer = displayTime;
        isDisplaying = true;
    }

    // 時間表示用
    public void ShowTime(int timeSeconds)
    {
        if (timeSeconds > 0)
            scoreText.text = "+" + timeSeconds + "sec";
        else
            scoreText.text = timeSeconds + "sec";
        timer = displayTime;
        isDisplaying = true;
    }
}
