using UnityEngine;
using System.Collections;

public class FeverManager : MonoBehaviour
{
    [Header("Fever Condition")]
    public int feverNeedScoreBall = 10; // 変更可能
    public static int scoreBallCount = 0;

    [Header("Fever Time")]
    public float feverDuration = 7f;

    [Header("Fever Text")]
    public GameObject feverTextPrefab;
    public GameObject feverYokoPrefab;
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
        if (isFever)
        {
            scoreBallCount = 0;
        }
    }

    // ==============================
    // Fever全体シーケンス
    // ==============================
    public IEnumerator FeverSequence()
    {
        isFever = true;


        // ① カットイン
        feverTextPrefab.SetActive(true);    
        StartFever();

        yield return new WaitForSeconds(feverDuration);
        EndFever();
        scoreBallCount = 0;
        feverTextPrefab.SetActive(false);
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
