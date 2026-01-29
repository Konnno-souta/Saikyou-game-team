using UnityEngine;
using UnityEngine.SceneManagement;

public class Enter : MonoBehaviour
{
    // ƒ{ƒ^ƒ“‚©‚çŒÄ‚Î‚ê‚éŠÖ”
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene("Player");
    }
}
