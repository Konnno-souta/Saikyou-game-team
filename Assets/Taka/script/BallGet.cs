using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BallGet : MonoBehaviour
{
    public Image showImage;
    public float displayTime = 3f;

    public Sprite speedupSprite;
    public Sprite speeddownSprite;
    public Sprite jumpupSprite;
    public Sprite jumpdownSprite;
    Coroutine currentCoroutine;

    void Start()
    {
        showImage.enabled = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        Sprite sprite = null;

        if (collision.gameObject.CompareTag("SpeedUp"))
            sprite = speedupSprite;
        else if (collision.gameObject.CompareTag("SpeedDown"))
            sprite = speeddownSprite;
        else if (collision.gameObject.CompareTag("JumpUp"))
            sprite = jumpupSprite;
        else if (collision.gameObject.CompareTag("JumpDown"))
            sprite = jumpdownSprite;
        if (sprite != null)
        {
            showImage.sprite = sprite;

            // òAë±éÊìæëŒçÙ
            if (currentCoroutine != null)
                StopCoroutine(currentCoroutine);

            currentCoroutine = StartCoroutine(ShowImageCoroutine());

            Destroy(collision.gameObject);
        }
    }

    IEnumerator ShowImageCoroutine()
    {
        showImage.enabled = true;
        yield return new WaitForSeconds(displayTime);
        showImage.enabled = false;
    }
}

