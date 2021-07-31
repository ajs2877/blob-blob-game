using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField]
    Slider volumeSlider;

    [SerializeField]
    AudioSource sound1;

    [SerializeField]
    AudioSource sound2;

    // Start is called before the first frame update
    void Start()
    {
        volumeSlider.value = sound1.volume;
    }

    public void ChangeVolume()
    {
        sound1.volume = volumeSlider.value;
        sound2.volume = volumeSlider.value;
    }
}
