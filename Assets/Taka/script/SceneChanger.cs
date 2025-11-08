using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // ‚±‚ÌŠÖ”‚ğƒ{ƒ^ƒ“‚É“o˜^‚·‚é
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
