using UnityEngine;
using UnityEngine.UI;

public class ScoreImageManager : MonoBehaviour
{
    public Sprite[] numberSprites; // 0～9 のスプライト
    public Image[] scoreImages;    // 左から右に4つ Image をセット [0]=千の位 ... [3]=1の位

    void Update()
    {
        UpdateScoreImage(ScoreManager.score);
    }

    void UpdateScoreImage(int score)
    {
        // スコアが負なら0にする
        if (score < 0) score = 0;

        // 4桁以上は切り捨て
        if (score > 9999) score = 9999;

        string scoreStr = score.ToString();

        // 全て非表示にする
        foreach (var img in scoreImages)
            img.enabled = false;

       

        // スコアの残り桁を右詰めで埋める（2桁目～左端）
        int imgIndex = scoreImages.Length - 1; // 右から2番目（10の位）
        for (int i = scoreStr.Length - 1; i >= 0 && imgIndex >= 0; i--, imgIndex--)
        {
            int num = scoreStr[i] - '0';
            if (num < 0 || num > 9) num = 0; // 安全策
            scoreImages[imgIndex].sprite = numberSprites[num];
            scoreImages[imgIndex].enabled = true;
        }
    }
}
