using UnityEngine;
using UnityEngine.UI;
public class ScoreManager : MonoBehaviour
{
    public static int score = 0;        // �X�R�A�����L�ł���悤�� static �ɂ���
    public Text scoreText;              // �X�R�A�\���p��Text(UI)
    public static int highScore = 0;    // �n�C�X�R�A�ۑ��p
    void Start()
    {
        score = 0;
        highScore = PlayerPrefs.GetInt("HighScore", 0); // �ۑ����ꂽ�n�C�X�R�A��ǂݍ���
        UpdateScoreText();
    }
    public void AddScore(int amount)
    {
        score += amount;
    }
    void Update()
    {
        UpdateScoreText();
        // �n�C�X�R�A�X�V�`�F�b�N
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