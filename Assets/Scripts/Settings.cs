using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;
using UnityEngine.Pool;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.Windows;

public class Settings : MonoBehaviour
{
    #region Singleton

    private static Settings _instance;

    public static Settings Instance
    {
        get
        {
            return _instance;
        }
        set
        {
            if(_instance != null)
            {
                Destroy(value.gameObject);
                return;
            }
            _instance = value;
        }
    }

    #endregion

    public Action onChangeResolution;

    [SerializeField] private Slider FramerateSlider;
    [SerializeField] public Slider SFXVolumeSlider;

    [SerializeField] private TMP_InputField FrameInput;
    [SerializeField] private TMP_InputField PooledObjectsInput;
    [SerializeField] public TMP_InputField SFXVolumeInput;

    [SerializeField] private TMP_Dropdown FullScreenSelection;
    [SerializeField] private TMP_Dropdown ResolutionSelection;

    [SerializeField] public AudioMixerGroup SFXMixer;

    private void Awake()
    {
        Instance = this;

        #region Framerate

        int framerate = PlayerPrefs.GetInt("MaxFramerate", 60);
        if(framerate == 0)
        {
            Application.targetFrameRate = -1;
        }
        else
        {
            Application.targetFrameRate = framerate;
        }
        FramerateSlider.value = framerate;
        FrameInput.text = framerate.ToString();

        #endregion

        #region FullScreenMode

        int fullScreenMode = PlayerPrefs.GetInt("FullScreenMode", 1);
        Screen.fullScreenMode = (FullScreenMode)fullScreenMode; 
        FullScreenSelection.ClearOptions();
        FullScreenSelection.AddOptions(Enum.GetNames(typeof(FullScreenMode)).ToList());
        FullScreenSelection.value = fullScreenMode;

        #endregion

        #region Resolution

        Resolution resolution = new Resolution();
        resolution.width = PlayerPrefs.GetInt("ResolutionWidth", 1920);
        resolution.height = PlayerPrefs.GetInt("ResolutionHeight", 1080);
        // Set res here
        ResolutionSelection.ClearOptions();
        List<string> resolutions = new List<string>();
        int usedResIndex = 0;
        foreach(Resolution res in Screen.resolutions)
        {
            resolutions.Add(res.ToString());
            if(res.width == resolution.width && res.height == resolution.height)
            {
                usedResIndex = resolutions.Count - 1;
            }
        }
        ResolutionSelection.AddOptions(resolutions);
        ResolutionSelection.value = usedResIndex;

        #endregion

        int poolSize = PlayerPrefs.GetInt("PooledObjects", 10000);
        PooledObjectsInput.text = poolSize.ToString();
    }

    public void ChangeSFXVolume(float volume)
    {
        SFXMixer.audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
        SFXVolumeInput.text = (Mathf.InverseLerp(SFXVolumeSlider.minValue, SFXVolumeSlider.maxValue, volume) * 100).ToString("N0");
    }

    public void ChangeSFXVolume(string volume)
    {
        int vol = Convert.ToInt32(volume);
        if(vol < 0) vol = 0;
        else if(vol > 100) vol = 100;
        SFXVolumeInput.text = vol.ToString("N0");

        float sliderValue = Mathf.Lerp(SFXVolumeSlider.minValue, SFXVolumeSlider.maxValue, vol / 100f);
        SFXMixer.audioMixer.SetFloat("SFXVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("SFXVolume", sliderValue);
        SFXVolumeSlider.value = sliderValue;
    }

    public void ChangePooledObjects(string input)
    {
        if(input == "0") input = "1";
        PlayerPrefs.SetInt("PooledObjects", Convert.ToInt32(input));

        GameManager.Instance.ChangePoolSize(Convert.ToInt32(input));
    }

    public void ChangeResolution(int index)
    {
        PlayerPrefs.SetInt("ResolutionWidth", Screen.resolutions[index].width);
        PlayerPrefs.SetInt("ResolutionHeight", Screen.resolutions[index].height);
        Screen.SetResolution(Screen.resolutions[index].width, Screen.resolutions[index].height, Screen.fullScreenMode);
        onChangeResolution?.Invoke();
    }

    public void ChangeFullScreenMode(int index)
    {
        Screen.fullScreenMode = (FullScreenMode)index; 
        PlayerPrefs.SetInt("FullScreenMode", index);
        onChangeResolution?.Invoke();
    }

    public void SetMaxFramerate(float framerate)
    {
        Application.targetFrameRate = (int)framerate;
        FrameInput.text = framerate.ToString();
        PlayerPrefs.SetInt("MaxFramerate", (int)framerate);
    }

    public void SetMaxFramerate(string framerate)
    {
        Application.targetFrameRate = Convert.ToInt32(framerate);
        FramerateSlider.value = Convert.ToInt32(framerate);
        PlayerPrefs.SetInt("MaxFramerate", Convert.ToInt32(framerate));
    }

    public void SaveMouseSensitivity(float sense)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", sense);
    }

    public void SaveSelectedSkin(int index)
    {
        PlayerPrefs.SetInt("SelectedSkin", index);
    }
}
