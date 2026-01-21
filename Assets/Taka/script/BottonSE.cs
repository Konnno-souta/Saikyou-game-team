using UnityEngine;

public class ButtonSE : MonoBehaviour
{
    private AudioSource audioSource=null;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySE(AudioClip clip)
    {
        if (audioSource !=null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.Log("ê›íËÇ≈Ç´ÇƒÇ‹ÇπÇÒ");
        }
    }
}
