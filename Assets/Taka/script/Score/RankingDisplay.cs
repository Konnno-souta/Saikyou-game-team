using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RankingDisplay : MonoBehaviour
{
    public TMP_Text rankingText;
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
        string display = "";

        for (int i = 0; i < rankingManager.maxRank; i++)
        {
            string colorHex = "#FFFFFF"; // デフォルト白

            switch (i)
            {
                case 0: colorHex = "#FFD700"; break; // 1位：ゴールド
                case 1: colorHex = "#C0C0C0"; break; // 2位：シルバー
                case 2: colorHex = "#CD7F32"; break; // 3位：ブロンズ
            }

            string scoreText = (i < ranking.Count) ? $"{ranking[i]}" : "-";
            //display += $"<color={colorHex}>{i + 1}位 : {scoreText}点</color>\n";
            display += $"<color={colorHex}>{i + 1}th : {scoreText} pts</color>\n";


        }

        rankingText.text = display;
    }
}
