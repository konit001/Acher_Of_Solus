using UI.Input;
using UnityEngine;
using UnityEngine.UI;

public class audioController : MonoBehaviour
{
    public Slider _musicSlider, _sfxSlider;

    public void ToggleMusic()

    {
        audioManager.Instance.ToggleMusic();
    }

    public void ToggleSFX()

    {
        audioManager.Instance.ToggleSFX();
    }

    public void MusicVolume()

    {
        audioManager.Instance.MusicVolume(_musicSlider.value);
    }

    public void SFXVolume()

    {
        audioManager.Instance.SFXVolume(_sfxSlider.value);
    }
}
