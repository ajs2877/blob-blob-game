using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField]
    Slider musicSlider;

    [SerializeField]
    Slider fxSlider;

    [SerializeField]
    AudioSource music;

    [SerializeField]
    AudioSource fx;

    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.HasKey("musicVolume"))
        {
            musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
            music.volume = musicSlider.value;
        }
        else
        {
            musicSlider.value = music.volume;
        }

        if (PlayerPrefs.HasKey("fxVolume"))
        {
            fxSlider.value = PlayerPrefs.GetFloat("fxVolume");
            fx.volume = fxSlider.value;
        }
        else
        {
            fxSlider.value = fx.volume;
        }
    }

    public void ChangeMusicVolume()
    {
        music.volume = musicSlider.value;
        PlayerPrefs.SetFloat("musicVolume", music.volume);
    }

    public void ChangeFXVolume()
    {
        fx.volume = fxSlider.value;
        PlayerPrefs.SetFloat("fxVolume", fx.volume);
    }
}
