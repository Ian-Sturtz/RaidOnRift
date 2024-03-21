using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class volumeslider : MonoBehaviour
{
    public Slider volumeSlider;
    // Start is called before the first frame update
    void Start()
    {

        if (!PlayerPrefs.HasKey("Volume"))
        {
            PlayerPrefs.SetFloat("Volume", 1);
            Load();
        }
        else
        {
            Load();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
        Save();
    }
    public void Save()
    {
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
    }
    public void Load()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("Volume");
        AudioListener.volume = volumeSlider.value;
    }
}