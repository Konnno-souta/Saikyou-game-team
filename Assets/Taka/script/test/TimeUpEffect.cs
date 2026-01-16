using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TimeUpCurtainEffect : MonoBehaviour
{
    [Header("Countdown")]
    public timetest countdown;  // タイマーからイベント受け取り

    [Header("UI Elements")]
    public Image timeUpImage;
    public RectTransform leftCurtain;
    public RectTransform rightCurtain;

    [Header("Settings")]
    public float curtainCloseDuration = 1f; // 閉じる時間（秒）

    private Vector2 leftStartPos;
    private Vector2 rightStartPos;
    private Vector2 leftEndPos;
    private Vector2 rightEndPos;

    private void Start()
    {
        timeUpImage.gameObject.SetActive(false);

        // 初期位置を記録
        leftStartPos = leftCurtain.anchoredPosition;
        rightStartPos = rightCurtain.anchoredPosition;

        // 画面中央で閉じる位置を計算
        float canvasWidth = ((RectTransform)leftCurtain.parent).rect.width;
        leftEndPos = new Vector2(-107, leftStartPos.y);
        rightEndPos = new Vector2(-1068, rightStartPos.y);

        // タイムアップイベントを購読
        countdown.OnTimeUp += StartCurtainSequence;
    }

    private void OnDestroy()
    {
        if (countdown != null)
            countdown.OnTimeUp -= StartCurtainSequence;
    }

    void StartCurtainSequence()
    {
        StartCoroutine(CurtainCloseRoutine());
    }

    IEnumerator CurtainCloseRoutine()
    {
        timeUpImage.gameObject.SetActive(true);

        float elapsed = 0f;

        Vector2 leftCurrent = leftStartPos;
        Vector2 rightCurrent = rightStartPos;

        while (elapsed < curtainCloseDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / curtainCloseDuration);
            t = Mathf.SmoothStep(0f, 1f, t); // Ease-In/Out

            leftCurtain.anchoredPosition = Vector2.Lerp(leftStartPos, leftEndPos, t);
            rightCurtain.anchoredPosition = Vector2.Lerp(rightStartPos, rightEndPos, t);

            yield return null;
        }

        // 最終位置を確定
        leftCurtain.anchoredPosition = leftEndPos;
        rightCurtain.anchoredPosition = rightEndPos;

        // スコア登録してリザルトへ
        RankingManager.Instance.AddScore(ScoreManager.score);
        SceneManager.LoadScene("ResultScene");
    }
}
