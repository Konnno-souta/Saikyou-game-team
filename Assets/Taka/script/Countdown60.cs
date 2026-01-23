
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Countdown60 : MonoBehaviour
{
    public Image tensImage;
    public Image onesImage;
    public Sprite[] numberSprites;

    public ScoreManager scoreManager;

    private int timeLeft = 60;
    private bool isBlinking = false;

    private Coroutine countdownCoroutine;
    internal bool isPaused;

    [SerializeField] FeverManager fivermanager;//Feverの情報取得用

    //internal static bool isPaused;

    public event Action OnTimeUp;
    void Start()
    {
        countdownCoroutine = StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        while (timeLeft > 0)
        {
            UpdateNumberImages(timeLeft);

            // 残り10秒で点滅
            if (timeLeft == 10 && !isBlinking)
            {
                StartCoroutine(BlinkNumbers());
            }

            yield return new WaitForSeconds(1f);
            if (!fivermanager.IsF)//Feverでないなら、カウントを進める
            {
                timeLeft--;
            }
        }

        // 0表示
        UpdateNumberImages(0);

        tensImage.enabled = false;
        onesImage.enabled = false;

        Debug.Log("Time up!");

        OnTimeUp?.Invoke();
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

        if (timeLeft > 10 && isBlinking)
        {
            isBlinking = false;
            tensImage.enabled = true;
            onesImage.enabled = true;
        }

        UpdateNumberImages(timeLeft);
    }
}