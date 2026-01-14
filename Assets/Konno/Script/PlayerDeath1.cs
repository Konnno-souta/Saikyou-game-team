using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath1 : MonoBehaviour
{
    // ゲームオーバーシーン名
    public string gameOverSceneName = "ResultScene";

    void OnCollisionEnter(Collision collision)
    {
        // 壁に当たったら
        if (collision.gameObject.CompareTag("Wall"))
        {
            Die();
            Debug.Log("死亡したよ");
        }
    }

    void Die()
    {
        // 必要ならSEや演出をここに追加
        SceneManager.LoadScene("ResultScene");
    }
}
