using UnityEngine;
using UnityEngine.UI;

public class ResultScore : MonoBehaviour
{
    public Text resultScoreText;
    public Text highScoreText;

    void Start()
    {
        resultScoreText.text = "Score: " + ScoreManager.score.ToString();
       // highScoreText.text = "HighScore: " + ScoreManager.highScore.ToString();
    }
}
