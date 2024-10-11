using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Slider slider;
    void Start()
    {
        if(!PlayerPrefs.HasKey("volume")){
            PlayerPrefs.SetFloat("volume", 1);
            Load();
        }
        else{
            Load();
        }
    }

    public void changeVolume(){
        AudioListener.volume = slider.value;
        Save();
    }

    public void Load(){
        slider.value = PlayerPrefs.GetFloat("volume");
    }

    public void Save(){
        PlayerPrefs.SetFloat("volume", slider.value);
    }
}
