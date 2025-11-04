using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class Countdown60 : MonoBehaviour
{
    public Image tensImage;        // 十の位用
    public Image onesImage;        // 一の位用
    public Sprite[] numberSprites; // 0〜9のスプライトを登録（配列サイズ10）
    private int timeLeft = 60;     // カウントダウンの秒数
    void Start()
    {
        StartCoroutine(Countdown());
    }
    IEnumerator Countdown()
    {
        while (timeLeft >= 0)
        {
            UpdateNumberImages(timeLeft);
            yield return new WaitForSeconds(1f);
            timeLeft--;
        }
        // カウント終了時の処理
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
}