using UnityEngine;
using UnityEngine.UI;

public class Tamaire : MonoBehaviour
{
    public ScoreManager scoreManager;
    [SerializeField] Playertap playertap;
    [SerializeField] Countdown60 countdown60;

    private bool ballCount;// ボールを数える用
    public bool bC { get { return ballCount; } }//ballCountを外部（他シート）から参照する用

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            // Ballスクリプトを取得
            Ball ball = other.GetComponent<Ball>();
            if (ball != null)
            {
                ballCount = true;
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
                    

                }

                    // スコアを追加
                    scoreManager.AddScore(score);

                // ボールを削除
                Destroy(other.gameObject);
            }
            else
            {
                ballCount = false;
            }
        }
    }
}
