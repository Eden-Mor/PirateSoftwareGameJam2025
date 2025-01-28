using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider carHonkSlider;

    private void Start()
    {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        SetMasterVolume(masterSlider.value);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        SetMusicVolume(musicSlider.value);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        SetSFXVolume(sfxSlider.value);
        carHonkSlider.value = PlayerPrefs.GetFloat("CarHonkVolume", 0.5f);
        SetCarHonkVolume(carHonkSlider.value);

        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        carHonkSlider.onValueChanged.AddListener(SetCarHonkVolume);
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
    }

    private void SetCarHonkVolume(float volume)
    {
        audioMixer.SetFloat("CarHonk", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("CarHonkVolume", carHonkSlider.value);
    }
}
