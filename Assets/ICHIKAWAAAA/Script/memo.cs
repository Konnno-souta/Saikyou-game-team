using UnityEngine;

public class Player : MonoBehaviour
{

    [Header("ステータス")]
    public float baseSpeed = 5f;
    public float speed = 5f;
    public float jump = 2f;


    private void ApplyBallEffect(string tagName)
    {
        switch (tagName)
        {
            case "SpeedUp":
                speed += 2f;
                Debug.Log("Speed Up! 現在のスピード: " + speed);
                break;

            case "SpeedDown":
                speed -= 2f;
                Debug.Log("Speed Down😢　現在のスピード:" + speed);
                break;

            case "JumpUp":
                jump += 1f;
                Debug.Log("Jump Up!!!! 現在のジャンプ力:" + jump);
                break;

            case "JumpDown":
                jump -= 1f;
                Debug.Log("Jump Down😢 現在のジャンプ力:" + jump);
                break;


            case "Invincible":
                // ２回攻撃防ぐ（デバフ）
                break;

            case "BigBasket":
                //　カゴ大きくなる
                break;

            case "MinusScore":
                //　スコアダウン
                break;

            case "MInusTime":
                //　タイム減る
                break;

            case "Bom":
                //　操作不可
                break;

            default:
                Debug.LogWarning("未対応のタグ: " + tagName);
                break;

        }
    }


    private void OnTriggerEnter(Collider other)
    {
        // タグで判定
        ApplyBallEffect(other.tag);

        // ボールを削除
        Destroy(other.gameObject);
    }

}