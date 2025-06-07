using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private Slider _SFXSlider;
    [SerializeField] private Slider _musicSlider;

    private void Start()
    {
        _SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        _musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        SetSFXVolumeLevel(_SFXSlider.value);
        SetMusicVolumeLevel(_musicSlider.value);       
    }

    public void SetSFXVolumeLevel(float volume)
    {
        _mixer.SetFloat("SFXVolume", volume);
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetMusicVolumeLevel(float volume)
    {
        _mixer.SetFloat("MusicVolume", volume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }
}

