using System;
using UnityEngine;

public class audioManager : MonoBehaviour
{
    public static audioManager Instance;
    public sound[] musicSound, sfxSound;
    public AudioSource musicSource, sfxSource;
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        PlayeMusic("Theme");
    }
    public void PlayeMusic(string name)
    {
        sound _sound = Array.Find(musicSound, x => x.name == name);

        if (_sound != null)
        {
            musicSource.clip = _sound.audioClip;
            musicSource.Play();
        }
    }

    public void PlayeSFX(string name)
    {
        sound _sound = Array.Find(sfxSound, x => x.name == name);

        if (_sound != null)
        {
            sfxSource.PlayOneShot(_sound.audioClip);
        }
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }

    public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
    }

    public void MusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
