using UnityEngine;
using UnityEngine.UI;

public class ResultScore : MonoBehaviour
{
    public Text resultScoreText;
    public Text redText;
    public Text greenText;
    public Text goldText;
    // public Text highScoreText;

    void Start()
    {
        resultScoreText.text = "Score: " + ScoreManager.score.ToString();
        // highScoreText.text = "HighScore: " + ScoreManager.highScore.ToString();
        redText.text = "Red: " + ScoreManager.redCount;
        greenText.text = "Green: " + ScoreManager.greenCount;
        goldText.text = "Gold: " + ScoreManager.goldCount;
    }
}
