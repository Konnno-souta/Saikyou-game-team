using UnityEngine;
using System.Collections;

public class FeverManager : MonoBehaviour
{
    [Header("Fever Condition")]
    public int feverNeedScoreBall = 7; // 変更可能
    public static int scoreBallCount = 0;

    [Header("Fever Time")]
    public float feverDuration = 7f;

    [Header("Fever Text")]
    public GameObject feverTextPrefab;
    public GameObject feverYokoPrefab;
    public Canvas canvas;

    [Header("Spot Fever Effect")]
    public SpotSub spotSub;

    [Header("BGM")]
    public AudioSource normalBGM;
    public AudioSource feverBGM;

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
        if (isFever) yield break;
        isFever = true;
        scoreBallCount = 0;

        // ① カットイン
        Instantiate(feverTextPrefab, canvas.transform);
        StartFever();
        yield return new WaitForSeconds(feverDuration);
        EndFever();
    }

    // ==============================
    void StartFever()
    {
        isPaused = true;

        if (spotSub != null)
            spotSub.SetSpotActive(true);

        StartCoroutine(BGMFade(normalBGM, 1f, 0f, 0.5f));
        StartCoroutine(BGMFade(feverBGM, 0f, 1f, 0.5f));
    }


    void EndFever()
    {
        isFever = false;
        isPaused = false;

        if (spotSub != null)
            spotSub.SetSpotActive(false);

        if(feverBGM != null)
        {
            feverBGM.Stop();
        }
    }

    IEnumerator BGMFade(AudioSource bgm, float from, float to, float time)
    {
        if (bgm == null) yield break;

        if (!bgm.isPlaying)
            bgm.Play();

        float t = 0f;
        bgm.volume = from;

        while (t < time)
        {
            t += Time.deltaTime;
            bgm.volume = Mathf.Lerp(from, to, t / time);
            yield return null;
        }

        bgm.volume = to;

        if (to == 0f)
            bgm.Pause(); // Stopより安全
    }

}
