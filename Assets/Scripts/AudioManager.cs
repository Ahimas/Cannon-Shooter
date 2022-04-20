using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip newWaveSound;
    [SerializeField] private AudioClip enemyCelebrating;
    [SerializeField] private AudioClip gameOverMusic;
    [SerializeField] private AudioClip enemyAppearenceSound; 

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.Play();
    }


    public void PlayExplosionSound()
    {
        audioSource.PlayOneShot(explosionSound);
    }

    public void PlayNewWaveSound()
    {
        audioSource.PlayOneShot(newWaveSound);
    }

    public void PlayGameOverMusic()
    {
        audioSource.clip = gameOverMusic;
        audioSource.Play();
    }

    public void PlayEnemyCelebrating()
    {
        audioSource.PlayOneShot(enemyCelebrating);
    }

    public void ChangeAudiVolume(float value)
    {
        audioSource.volume = value;
    }

    public void PlayEnemyAppearenceSound()
    {
        audioSource.PlayOneShot(enemyAppearenceSound);
    }
}
