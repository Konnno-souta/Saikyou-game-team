using UnityEngine;
using UnityEngine.SceneManagement;

public class Enter : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ChangeScene();
        }
    }

    // ƒ{ƒ^ƒ“‚©‚ç‚àŒÄ‚×‚é
    public void ChangeScene()
    {
        SceneManager.LoadScene("Player");
    }
}
