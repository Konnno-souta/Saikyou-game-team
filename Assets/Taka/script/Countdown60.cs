
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Countdown60 : MonoBehaviour
{
    public Image tensImage;
    public Image onesImage;
    public Sprite[] numberSprites;

    private int timeLeft = 60;
    private bool isBlinking = false;

    private Coroutine countdownCoroutine;

    void Start()
    {
        countdownCoroutine = StartCoroutine(Countdown());
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

        while (timeLeft > 0)
        {
            tensImage.enabled = false;
            onesImage.enabled = false;
            yield return new WaitForSeconds(0.3f);

            tensImage.enabled = true;
            onesImage.enabled = true;
            yield return new WaitForSeconds(0.3f);
        }

        isBlinking = false;
    }

    
    public void AddTime(int addSeconds)
    {
        timeLeft += addSeconds;

        // 追加後に 10秒以上になったら点滅を止める
        if (timeLeft > 10 && isBlinking)
        {
            isBlinking = false;
            tensImage.enabled = true;
            onesImage.enabled = true;
        }

        // 表示を即更新
        UpdateNumberImages(timeLeft);
    }
}
