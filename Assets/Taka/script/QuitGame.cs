
using UnityEngine;
using System.Collections;

public class QuitGame : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip quitSE;

    public void Quit()
    {
        // SEを鳴らしてから終了するコルーチンを呼ぶ
        StartCoroutine(PlaySEAndQuit());
    }

    private IEnumerator PlaySEAndQuit()
    {
        if (audioSource != null && quitSE != null)
        {
            audioSource.PlayOneShot(quitSE);
            yield return new WaitForSeconds(quitSE.length);
        }
        else
        {
            Debug.LogWarning("AudioSource または AudioClip が設定されていません");
        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
