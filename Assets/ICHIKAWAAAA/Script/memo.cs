//using UnityEngine;
//using static Ball;

//public class Player : MonoBehaviour
//{
//    public int power = 0;
//    public float speed = 5f;
//    public int hp = 100;

//    private void OnTriggerEnter(Collider other)
//    {
//        Ball ball = other.GetComponent<Ball>();
//        if (ball != null && !ball.isCollected)
//        {
//            ball.isCollected = true;
//            ApplyBallEffect(ball.ballType);
//            Destroy(ball.gameObject);
//        }
//    }

//    private void ApplyBallEffect(BallType type)
//    {
//        switch (type)
//        {
//            case BallType.PowerUp:
//                power++;
//                Debug.Log("Power Up! 現在のパワー: " + power);
//                break;
//            case BallType.SpeedUp:
//                speed += 2f;
//                Debug.Log("Speed Up! 現在のスピード: " + speed);
//                break;
//            case BallType.Heal:
//                hp += 20;
//                Debug.Log("Heal! 現在のHP: " + hp);
//                break;
//            case BallType.Special:
//                Debug.Log("Special Effect 発動！");
//                // 特殊処理
//                break;
//        }
//    }
//}