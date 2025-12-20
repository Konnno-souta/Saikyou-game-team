using UnityEngine;
using UnityEngine.UI;

public class Tamaire : MonoBehaviour
{
    public ScoreManager scoreManager;
    //[SerializeField] Playermain playermain;
    [SerializeField] Countdown60 countdown60;

    private bool ballCount;// ボールを数える用
    public bool bC { get { return ballCount; } }//ballCountを外部（他シート）から参照する用

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Trigger Hit: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Ball"))
        {
            Debug.Log("Ball detected");
            // Ballスクリプトを取得
            bbbb ball = collision.gameObject.GetComponent<bbbb>();
            Debug.Log(ball);
            if (ball != null)
            {
                //ballCount = true;
                int score = 0;//ここを消す

                // ボールの種類によってスコアを変更
                switch (ball.ballType)
                {
                    case bbbb.BallType.Green:
                        score = 10;
                        break;
                    case bbbb.BallType.Red:
                        score = 30;
                        break;
                    case bbbb.BallType.Gold:
                        score = 50;
                        break;
                    //case Ball.BallType.SpeedUp:
                    //    playermain.SpeedUp(3f, 5f); // ← 例: +3 のスピードを 5 秒間
                        break;
                    case bbbb.BallType.Time:       // ← 追加！！
                        countdown60.AddTime(5);      // 5 秒追加
                        break;
                    

                }

                    // スコアを追加
                    scoreManager.AddScore(score);//ここを消す

                // ボールを削除
                Destroy(collision.gameObject.gameObject);
            }
            //else
            //{
            //    ballCount = false;
            //}
        }
    }
}
