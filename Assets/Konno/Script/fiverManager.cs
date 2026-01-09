using UnityEngine;
using System.Collections;

public class FeverManager : MonoBehaviour
{
    [Header("Fever Condition")]
    public int feverNeedScoreBall = 10; // 変更可能
    int scoreBallCount;

    [Header("Fever Time")]
    public float feverDuration = 7f;

    [Header("Fever Text")]
    public GameObject feverTextPrefab;
    public Canvas canvas;

    [Header("Ball Control")]
    public BallSpawner normalSpawner;
    public BallSpawner feverSpawner;   // 金ボール専用

    [Header("Pause Targets")]
    public Countdown60 Timer;
    public ScrollDirectionSet Scroll;
    public SpawnManager Spawner;

    bool isFever;
    public bool IsFever => isFever;
    public bool IsF { get { return isFever; } } // 他スクリプト用（プロパティ）

    // ==============================
    // スコアボール取得時に呼ぶ
    // ==============================
    public void OnCatchScoreBall()
    {
        if (isFever) return;

        scoreBallCount++;

        if (scoreBallCount >= feverNeedScoreBall)
        {
            StartCoroutine(FeverSequence());
        }
    }

    // ==============================
    // Fever全体シーケンス
    // ==============================
    IEnumerator FeverSequence()
    {
        isFever = true;
        scoreBallCount = 0;

        // ① カットイン
        Instantiate(feverTextPrefab, canvas.transform);

        // 少し待ってからFever突入（演出用）
        yield return new WaitForSeconds(0.5f);

        StartFever();
        yield return new WaitForSeconds(feverDuration);
        EndFever();
    }

    // ==============================
    void StartFever()
    {
        // 通常生成停止
        normalSpawner.isPaused = true;

        // Fever生成開始
        feverSpawner.isPaused = false;

        // 他システム停止
        Timer.isPaused = true;
        Scroll.isPaused = true;
        Spawner.isPaused = true;
    }

    void EndFever()
    {
        isFever = false;

        normalSpawner.isPaused = false;
        feverSpawner.isPaused = true;

        Timer.isPaused = false;
        Scroll.isPaused = false;
        Spawner.isPaused = false;
    }
}
