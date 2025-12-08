using UnityEngine;
using UnityEngine.SceneManagement; // シーンをリロードするため

public class PlayerDeath : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        // ボールにぶつかったら
        if (collision.gameObject.CompareTag("Ball"))
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("プレイヤー死亡！");
        // プレイヤーを消す
        gameObject.SetActive(false);

        // 2秒後にシーンをリスタート
        Invoke("RestartScene", 2f);
    }

    void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
