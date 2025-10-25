using UnityEngine;
using UnityEngine.UI;
public class ScoreManager : MonoBehaviour
{
    public static int score = 0;        // スコアを共有できるように static にする
    public Text scoreText;              // スコア表示用のText(UI)
    public static int highScore = 0;    // ハイスコア保存用
    void Start()
    {
        score = 0;
        highScore = PlayerPrefs.GetInt("HighScore", 0); // 保存されたハイスコアを読み込む
        UpdateScoreText();
    }
    public void AddScore(int amount)
    {
        score += amount;
    }
    void Update()
    {
        UpdateScoreText();
        // ハイスコア更新チェック
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }
    }
    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score.ToString() + "\nHighScore: " + highScore.ToString();
    }
}