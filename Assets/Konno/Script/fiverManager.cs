using UnityEngine;
using System.Collections;

public class fiverManager : MonoBehaviour
{
    [Header("Fever Condition")]
    public int feverNeedScoreBall = 10; // 変更可能
    public static int scoreBallCount = 0;

    [Header("Fever Time")]
    public float feverDuration = 7f;

    [Header("Fever Text")]
    public GameObject feverTextPrefab;
    public Canvas canvas;

    bool isPaused;

    public bool IsP { get { return isPaused; } }

    bool isFever;
    public bool IsF { get { return isFever; } } // 他スクリプト用（プロパティ）

    private void Start()
    {
        isFever = false;
        isPaused = false;
        scoreBallCount = 0;
    }

    void FixedUpdate()
    {
        if (scoreBallCount >= feverNeedScoreBall)
        {
            StartCoroutine(FeverSequence());
            Debug.Log(scoreBallCount);
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
        isPaused = true;
        
    }

    void EndFever()
    {
        isFever = false;

        isPaused = false;
    }
}
