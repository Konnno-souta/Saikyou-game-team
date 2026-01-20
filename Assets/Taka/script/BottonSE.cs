using UnityEngine;

public class ButtonSE : MonoBehaviour
{
    public AudioSource audioSource;

    public void PlaySE(AudioClip clip)
    {
        Debug.Log("Button clicked");
        audioSource.PlayOneShot(clip);
    }
}
