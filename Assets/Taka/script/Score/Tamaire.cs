using UnityEngine;
using UnityEngine.UI;


public class Tamaire : MonoBehaviour
{

    public ScoreManager scoreManager;
    //[SerializeField] Playermain playermain;
    [SerializeField] Countdown60 countdown60;
    public ScrollDirectionSet scrollDirectionSet;//スクロール管理のスクリプトを持ってくる
    public FeverManager feverManager;
    //[SerializeField] private GameObject obj;
   


    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Trigger Hit: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Ball"))
        {
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
                        ScrollDirectionSet.ballCount++;
                        ScrollDirectionSet.ballCount2+=10; //スクロールで準備してある変数に＋する
                        FeverManager.scoreBallCount++;
                        break;
                    case ball.BallType.Red:
                        score = 30;
                        ScoreManager.AddRed();                  
                        ScrollDirectionSet.ballCount++;
                        ScrollDirectionSet.ballCount2+=30;
                        FeverManager.scoreBallCount++;
                        break;
                    case ball.BallType.Gold:
                        score = 50;
                        ScoreManager.AddGold();
                        ScrollDirectionSet.ballCount++;
                        ScrollDirectionSet.ballCount2+=50;//同上
                        break;
                    case ball.BallType.Time:       // ← 追加！！
                        countdown60.AddTime(5);      // 5 秒追加
                        break;
                    case ball.BallType.Minus:
                        score = -30;                 // ← マイナス30点
                        ScoreManager.AddMinus();                         // 必要ならカウントしない／別処理も可能
                        break;
                    case ball.BallType.Minustime:       // ← 追加！！
                   
                        FindObjectOfType<Countdown60>().AddTime(-5);
                        break;
                    //case ball.BallType.Bom:
                    //    // ▼ プレイヤーを探して Bom 効果発動
                    //    PlayerSideSlide player = FindObjectOfType<PlayerSideSlide>();
                    //    if (player != null)
                    //    {
                    //        player.ApplyBallEffect("Bom");
                    //    }
                    //    break;




                }

                // スコアを追加
                scoreManager.AddScore(score);

                
                // ボールを削除
                Destroy(collision.gameObject.gameObject);
            }
        }
    }
}
