using UnityEngine;
using UnityEngine.UI; // UI ImageÇégÇ§èÍçá

public class BallImageChanger : MonoBehaviour
{
    public Image displayImage;   // UI Image
    public Sprite redBallSprite;
    public Sprite blueBallSprite;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RedBall"))
        {
            displayImage.sprite = redBallSprite;
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("BlueBall"))
        {
            displayImage.sprite = blueBallSprite;
            Destroy(other.gameObject);
        }
    }
}
