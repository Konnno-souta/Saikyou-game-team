using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RankingDisplay : MonoBehaviour
{
    public TMP_Text[] rankingTexts;   // 10個
    public RankingManager rankingManager;

    private void Start()
    {
        UpdateRankingDisplay();
    }

    public void UpdateRankingDisplay()
    {
        if (rankingManager == null)
        {
            Debug.LogError("RankingManager が設定されていません");
            return;
        }

        List<int> ranking = rankingManager.GetRanking();

        for (int i = 0; i < rankingTexts.Length; i++)
        {
            string colorHex = "#FFFFFF";

            switch (i)
            {
                case 0: colorHex = "#FFD700"; break;
                case 1: colorHex = "#C0C0C0"; break;
                case 2: colorHex = "#CD7F32"; break;
            }

            string scoreText = (i < ranking.Count) ? ranking[i].ToString() : "-";

            // 1〜3位はスコアのみ表示
            if (i < 3)
            {
                rankingTexts[i].text =
                    $"<color={colorHex}>{scoreText} pts</color>";
            }
            else
            {
                rankingTexts[i].text =
                    $"<color={colorHex}>{i + 1} : {scoreText} pts</color>";
            }

        }
    }
}
