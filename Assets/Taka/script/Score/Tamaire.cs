using UnityEngine;
using UnityEngine.UI;

public class Tamaire : MonoBehaviour
{
    public ScoreManager scoreManager;
    //[SerializeField] Playermain playermain;
    [SerializeField] Countdown60 countdown60;
    public ScrollDirectionSet scrollDirectionSet;//スクロール管理のスクリプトを持ってくる
    //[SerializeField] private GameObject obj;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Trigger Hit: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Ball"))
        {
            //scrollDirectionSet = obj.GetComponent<ScrollDirectionSet>();    
            Debug.Log("Ball detected");
            // Ballスクリプトを取得
            ball ball = collision.gameObject.GetComponent<ball>();
            if (ball != null)
                
            {
                int score = 0;//ここを消す
                //if (scrollDirectionSet == null)
                //{
                //    Debug.LogError("ScrollDirectionSet が Inspector で設定されていません");
                //    return;
                //}

                // ボールの種類によってスコアを変更
                switch (ball.ballType)
                {
                    case ball.BallType.Green:
                        score = 10;
                        ScoreManager.AddGreen();
                        //ScrollDirectionSet.ballCount++;
                        //ScrollDirectionSet.ballCount2++;  //スクロールで準備してある変数に＋１する
                        break;
                    case ball.BallType.Red:
                        score = 30;
                        ScoreManager.AddRed();
                        ScrollDirectionSet.ballCount++;
                        ScrollDirectionSet.ballCount2++;
                        break;
                    case ball.BallType.Gold:
                        score = 50;
                        ScoreManager.AddGold();
                        //ScrollDirectionSet.ballCount++;
                        //ScrollDirectionSet.ballCount2++;//同上
                        break;
                    //case Ball.BallType.SpeedUp:
                    //    playermain.SpeedUp(3f, 5f); // ← 例: +3 のスピードを 5 秒間
                    
                    case ball.BallType.Time:       // ← 追加！！
                        countdown60.AddTime(5);      // 5 秒追加
                        break;
                    

                }

                    // スコアを追加
                    scoreManager.AddScore(score);
               
                // ボールを削除
                Destroy(collision.gameObject.gameObject);
            }
        }
    }
}
