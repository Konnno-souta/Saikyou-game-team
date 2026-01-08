using System.Collections.Generic;
using UnityEngine;

public class RankingManager : MonoBehaviour
{
    public static RankingManager Instance;

    public int maxRank = 10;
    public List<int> rankingScores = new List<int>();

    private void Awake()
    {
        // シングルトン
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadRanking();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ランキングにスコアを追加
    /// </summary>
    public void AddScore(int score)
    {
        rankingScores.Add(score);

        // 降順ソート（高い順）
        rankingScores.Sort((a, b) => b.CompareTo(a));

        // 上位 maxRank 件だけ残す
        if (rankingScores.Count > maxRank)
        {
            rankingScores.RemoveAt(rankingScores.Count - 1);
        }

        SaveRanking();
    }

    /// <summary>
    /// ランキング取得
    /// </summary>
    public List<int> GetRanking()
    {
        return rankingScores;
    }

    void SaveRanking()
    {
        PlayerPrefs.SetInt("RankingCount", rankingScores.Count);

        for (int i = 0; i < rankingScores.Count; i++)
        {
            PlayerPrefs.SetInt($"RankingScore_{i}", rankingScores[i]);
        }

        PlayerPrefs.Save();
    }

    void LoadRanking()
    {
        rankingScores.Clear();

        int count = PlayerPrefs.GetInt("RankingCount", 0);

        for (int i = 0; i < count; i++)
        {
            rankingScores.Add(PlayerPrefs.GetInt($"RankingScore_{i}"));
        }
    }

    /// <summary>
    /// ランキング初期化（デバッグ用）
    /// </summary>
    public void ResetRanking()
    {
        PlayerPrefs.DeleteAll();
        rankingScores.Clear();
    }
}
