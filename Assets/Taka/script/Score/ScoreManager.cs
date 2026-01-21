using UnityEngine;
using UnityEngine.UI;
public class ScoreManager : MonoBehaviour
{
    public static int score = 0;        // スコアを共有できるように static にする
    public Text scoreText;              // スコア表示用のText(UI)
    public static int highScore = 0;    // ハイスコア保存用
    public static int redCount = 0;
    public static int greenCount = 0;
    public static int goldCount = 0;
    void Start()
    {
        score = 0;
        redCount = 0;
        greenCount = 0;
        goldCount= 0;
        highScore = PlayerPrefs.GetInt("HighScore", 0); // 保存されたハイスコアを読み込む
        UpdateScoreText();
    }
    public void AddScore(int value)
    {
        
        score += value;

        // 0 未満にならないようにする
        if (score < 0)
        {
            score = 0;
        }
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
    public static void AddRed()
    {
        redCount++;
    }

    public static void AddGreen()
    {
        greenCount++;
    }
    public static void AddGold()
    {
        goldCount++;
    }

    public static void AddMinus()
    {
    }
    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score.ToString() + "\nHighScore: " + highScore.ToString()+ "\nRed: " + redCount +
        "\nGreen: " + greenCount+ "\nGold: " + goldCount; ;
    }
   

   
}