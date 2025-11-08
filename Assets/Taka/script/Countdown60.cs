
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Countdown60 : MonoBehaviour
{
    public Image tensImage;        // 十の位用
    public Image onesImage;        // 一の位用
    public Sprite[] numberSprites; // 0〜9のスプライトを登録（配列サイズ10）

    private int timeLeft = 15;     // カウントダウンの秒数
    private bool isBlinking = false; // 点滅中フラグ

    void Start()
    {
        StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        while (timeLeft >= 0)
        {
            UpdateNumberImages(timeLeft);

            // 残り10秒になったら点滅開始
            if (timeLeft == 10 && !isBlinking)
            {
                StartCoroutine(BlinkNumbers());
            }

            yield return new WaitForSeconds(1f);
            timeLeft--;
        }

        // カウント終了時
        tensImage.enabled = false;
        onesImage.enabled = false;
        Debug.Log("Time up!");
    }

    void UpdateNumberImages(int number)
    {
        int tens = number / 10;
        int ones = number % 10;
        tensImage.sprite = numberSprites[tens];
        onesImage.sprite = numberSprites[ones];
    }

    IEnumerator BlinkNumbers()
    {
        isBlinking = true;

        while (timeLeft > 0) // 残り0になるまで点滅
        {
            // 画像を非表示に
            tensImage.enabled = false;
            onesImage.enabled = false;
            yield return new WaitForSeconds(0.3f);

            // 再表示
            tensImage.enabled = true;
            onesImage.enabled = true;
            yield return new WaitForSeconds(0.3f);
        }

        isBlinking = false;
    }
}
