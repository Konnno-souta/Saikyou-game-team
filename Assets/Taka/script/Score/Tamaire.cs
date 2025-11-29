using UnityEngine;

public class Tamaire : MonoBehaviour
{
    public ScoreManager scoreManager;
    [SerializeField] Playertap playertap;
    [SerializeField] Countdown60 countdown60;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            // Ballスクリプトを取得
            Ball ball = other.GetComponent<Ball>();
            if (ball != null)
            {
                int score = 0;

                // ボールの種類によってスコアを変更
                switch (ball.ballType)
                {
                    case Ball.BallType.Green:
                        score = 10;
                        break;
                    case Ball.BallType.Red:
                        score = 30;
                        break;
                    case Ball.BallType.Gold:
                        score = 50;
                        break;
                    case Ball.BallType.SpeedUp:
                        playertap.SpeedUp(3f, 5f); // ← 例: +3 のスピードを 5 秒間
                        break;
                    case Ball.BallType.Time:       // ← 追加！！
                        countdown60.AddTime(5);      // 5 秒追加
                        break;
                    //case Ball.BallType.Bomb:         // ← 追加！
                    //    playertap.Bomb(2f);        // 2秒間動けない
                    //    break;

                }

                    // スコアを追加
                    scoreManager.AddScore(score);

                // ボールを削除
                Destroy(other.gameObject);
            }
        }
    }
}
