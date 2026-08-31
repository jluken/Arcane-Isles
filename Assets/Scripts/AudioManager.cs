using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip sandStep;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySandStep()
    {
        audioSource.clip = sandStep;
        audioSource.volume = 0.5f;
        audioSource.Play();
    }
}
