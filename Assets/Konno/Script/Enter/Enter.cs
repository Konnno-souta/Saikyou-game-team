using UnityEngine;
using UnityEngine.SceneManagement;

public class Enter : MonoBehaviour
{
    [SerializeField] AudioSource seSource; // SE—p
    [SerializeField] AudioClip enterSE;    // Ä¶‚·‚éSE

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
        if (seSource != null && enterSE != null)
        {
            seSource.PlayOneShot(enterSE);
        }

        SceneManager.LoadScene("Player");
    }
}
