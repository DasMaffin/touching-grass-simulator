using DigitalRuby.RainMaker;
using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    #region Singleton

    private static WeatherManager _instance;

    public static WeatherManager Instance
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

    // rs.RainIntensity:
    // 0.0 - 0.32999... = Light
    // 0.33 - 0,66999... = medium
    // 0.67 - 1 = Heavy
    [SerializeField] public RainScript rs;

    /// <summary>
    /// Arg1: OldWeather
    /// Arg2: NewWeather
    /// </summary>
    private Action<Weather, Weather> OnWeatherChanged;
    private Weather weather;
    public Weather Weather
    {
        get => weather;
        set
        {
            OnWeatherChanged?.Invoke(weather, value);
            weather = value;
        }
    }

    // If not watered
    // 0 - 0.5 = 1x - 10x growth
    // 0.5 - 0.75 = 10x - 1x growth
    // 0.75 - 0.9 = 1x - 0x growth
    // > 0.9 drown
    // If watered
    // 0 - 0.3299.. = 1x
    // 0.33 - 0.6699 = 1x - 0x
    // > 0.67 = drown
    public float GetGrowthMultiplier(bool isWatered, float rainIntensity = -1)
    {
        if(rainIntensity == -1) rainIntensity = rs.RainIntensity;

        if(isWatered)
        {
            if(rainIntensity > 0.67f) return -1f; // Drown
            if(rainIntensity > 0.33f) return 1f - ((rainIntensity - 0.33f) / 0.3399f); // 1x to 0x growth
            return 1f; // Constant 1x growth
        }
        else
        {
            if(rainIntensity > 0.9f) return -1f; // Drown
            if(rainIntensity > 0.75f) return (0.9f - rainIntensity) / 0.15f; // 1x to 0x growth
            if(rainIntensity > 0.5f) return 1f + (10f - 1f) * ((0.75f - rainIntensity) / 0.25f); // 10x to 1x growth
            return 1f + (10f - 1f) * (rainIntensity / 0.5f); // 1x to 10x growth
        }
    }


    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Weather = Weather.Sunny;
        InvokeRepeating("ChangeWeather", 360, 360);
        OnWeatherChanged += OnChangeWeather;
    }

    private void OnChangeWeather(Weather oldWeather, Weather newWeather)
    {
        switch(newWeather)
        {
            case Weather.Sunny:
                rs.RainIntensity = 0;
                break;
            case Weather.LightRain:
                rs.RainIntensity = UnityEngine.Random.Range(0f, 0.3299f);
                break;
            case Weather.MediumRain:
                rs.RainIntensity = UnityEngine.Random.Range(0.33f, 0.6699f);
                break;
            case Weather.HeavyRain:
                rs.RainIntensity = UnityEngine.Random.Range(0.67f, 1);
                break;
            default:
                break;
        }
    }

    [ContextMenu("Change Weather")]
    private void ChangeWeather()
    {
        float chance = UnityEngine.Random.Range(0f, 100f);
        switch(Weather)
        {
            case Weather.Sunny:
                if(chance < 50) Weather = Weather.LightRain;
                break;
            case Weather.LightRain:
                if(chance < 33.3f)
                    Weather = Weather.MediumRain;
                else if(chance < 66.6f)
                    Weather = Weather.Sunny;
                break;
            case Weather.MediumRain:
                if(chance < 60)
                    Weather = Weather.LightRain;
                else if(chance < 80)
                    Weather = Weather.HeavyRain;
                break;
            case Weather.HeavyRain:
                if(chance < 80)
                    Weather = Weather.MediumRain;
                break;
            default:
                break;
        }
    }
}
