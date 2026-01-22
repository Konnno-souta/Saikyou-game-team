

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneButtonController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip titleSE;
    public AudioClip startSE;
    public AudioClip resultSE;

    public void GoTitle()
    {
        StartCoroutine(PlaySEAndLoadScene(titleSE, "TitleScene"));
    }

    public void GoPlayer()
    {
        StartCoroutine(PlaySEAndLoadScene(startSE, "Setumei"));
    }

    public void GoResult()
    {
        StartCoroutine(PlaySEAndLoadScene(resultSE, "Result"));
    }

    private IEnumerator PlaySEAndLoadScene(AudioClip clip, string sceneName)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
            yield return new WaitForSeconds(clip.length);
        }
        else
        {
            Debug.LogWarning("AudioClip Ç‹ÇΩÇÕ AudioSource Ç™ê›íËÇ≥ÇÍÇƒÇ¢Ç‹ÇπÇÒ");
        }

        SceneManager.LoadScene(sceneName);
    }

}
