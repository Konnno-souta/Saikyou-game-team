using UnityEngine;

public class BottomScoreTrigger : MonoBehaviour
{
    public ScoreManager scoreManager;

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
                }

                // スコアを追加
                scoreManager.AddScore(score);

                // ボールを削除
                Destroy(other.gameObject);
            }
        }
    }
}
