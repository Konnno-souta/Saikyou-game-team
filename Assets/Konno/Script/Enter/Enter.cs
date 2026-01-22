using UnityEngine;
using UnityEngine.SceneManagement;

public class Enter : MonoBehaviour
{
    [Header("メインシーン")]
    public string nextSceneName = "Player";

    void Update()
    {
        // Enterキーが押されたら
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("Player");
        }
    }
}
