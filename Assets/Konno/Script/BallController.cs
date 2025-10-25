using UnityEngine;

public class BallController : MonoBehaviour
{
    void Start()
    {
        // ボールを5秒後に自動削除（落ちすぎ防止）
        Destroy(gameObject, 5f);
    }

    void OnCollisionEnter(Collision collision)
    {
        // プレイヤーにぶつかったら死亡
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("プレイヤーに命中！");
            collision.gameObject.SendMessage("Die", SendMessageOptions.DontRequireReceiver);
            Destroy(gameObject); // ボールも消す
        }
    }
}
