using UnityEngine;
using System;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEngine.Windows;
using Unity.Collections;
using Unity.Jobs;

public class Player
{
    public event Action<float> OnMoneyChanged;
    private float money = 0;
    public float Money
    {
        get { return money; }
        set
        {
            money = value;
            OnMoneyChanged?.Invoke(money); // Trigger event
        }
    }

    public event Action<float> OnAvailableWaterChanged;
    private float availableWater;
    public float AvailableWater
    {
        get
        {
            return availableWater;
        }
        set
        {
            availableWater = value;
            OnAvailableWaterChanged?.Invoke(availableWater);
        }
    }


    public bool IsWatering = false;
    public void UseWateringCan()
    {
        if(IsWatering)
        {
            if(AvailableWater <= 0)
            {
                PlayerController.Instance.StopUsingWateringCan();
            }
            AvailableWater -= 1f * Time.deltaTime;
        }
    }
}

public class GameManager : MonoBehaviour, IDataPersistence
{
    #region Singleton

    private static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
        set
        {
            if(instance != null)
            {
                Destroy(value.gameObject);
                return;
            }
            instance = value;
        }
    }

    #endregion

    public InteractiveTerrainTexture ITT;
    public Stack<GameObject> menuHistory = new Stack<GameObject>();
    public Player player;
    public GameObject[] grassSkins;
    public Dictionary<Item, Sprite> itemSpriteMap;
    public List<ObjectPool<GameObject>> grassPools = new List<ObjectPool<GameObject>>();
    public List<GrassBladeController> activeGrassBlades = new List<GrassBladeController>();

    [HideInInspector] public int selectedGrassSkin;

    [SerializeField] private Sprite noneSprite;
    [SerializeField] private Sprite seedsSprite;
    [SerializeField] private Sprite bladesSprite;
    [SerializeField] private Sprite wateringCanSprite;

    private Dictionary<GameObject, GrassBladeController> grassCache = new Dictionary<GameObject, GrassBladeController>();

    void Awake()
    {
        instance = this;
        player = new Player();
        itemSpriteMap = new Dictionary<Item, Sprite>
        {
            { Item.None, noneSprite },
            { Item.GrassSeeds, seedsSprite },
            { Item.GrassBlades, bladesSprite },
            { Item.WateringCan, wateringCanSprite }
        };
    }

    private void Start()
    {
        player.AvailableWater = 100f;

        #region SFXVolume

        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", .5f);
        print(sfxVolume);
        Settings.Instance.SFXMixer.audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);
        Settings.Instance.SFXVolumeSlider.value = sfxVolume;
        Settings.Instance.SFXVolumeInput.text = (Mathf.InverseLerp(Settings.Instance.SFXVolumeSlider.minValue, Settings.Instance.SFXVolumeSlider.maxValue, sfxVolume) * 100).ToString("N0");

        #endregion
    }

    public void ChangePoolSize(int input)
    {
        foreach(var pool in grassPools)
        {
            pool.Clear(); // Ensure Unity's ObjectPool clears its stored instances
        }
        grassPools.Clear();

        for(int i = 0; i < grassSkins.Length; i++)
        {
            int index = i;
            grassPools.Add(new ObjectPool<GameObject>(
                () =>
                {
                    GameObject go = Instantiate(grassSkins[index]);
                    GrassBladeController gbc = go.GetComponent<GrassBladeController>();
                    gbc.selectedGrassSkin = index;
                    grassCache[go] = gbc;
                    return go;
                },
                go =>
                {
                    activeGrassBlades.Add(grassCache[go]);
                    go.SetActive(true);
                },
                go =>
                {
                    if(!grassCache.TryGetValue(go, out GrassBladeController gbc))
                    {
                        gbc = go.GetComponent<GrassBladeController>();
                        grassCache.Add(go, gbc);
                    }
                    gbc.watered = false;
                    gbc.currentSize = 0.01f;
                    gbc.daisies = 0;
                    gbc.OnHoverExit();
                    go.transform.localScale = new Vector3(gbc.currentSize, gbc.currentSize, gbc.currentSize);
                    activeGrassBlades.Remove(grassCache[go]);
                    go.SetActive(false);
                },
                go => Destroy(go),
            false,
                input,
                input * 10
            ));

            for(int j = 0; j < input; j++)
            {
                GameObject go = Instantiate(grassSkins[index]);
                GrassBladeController gbc = go.GetComponent<GrassBladeController>();
                gbc.selectedGrassSkin = index;
                grassCache[go] = gbc;
                grassPools[i].Release(go);
            }
        }
    }

    private void Update()
    {
        player.UseWateringCan();
    }

    internal GameObject InstantiateGrass(Vector3 location)
    {
        GameObject grass = grassPools[selectedGrassSkin].Get();
        grass.transform.position = location;
        grass.transform.rotation = Quaternion.identity;
        return grass;
    }

    public void LoadData(GameData data)
    {
        this.player.Money = data.money;
        this.player.AvailableWater = 100f; // TODO data.water;
        foreach(GameData.GrassBladeData gbd in data.grassPlants)
        {
            GrassBladeController gbc = InstantiateGrass(gbd.location).GetComponent<GrassBladeController>();
            gbc.currentSize = gbd.currentSize;
        }
    }

    public void SaveData(ref GameData data)
    {
        data.money = this.player.Money;
        data.water = this.player.AvailableWater;
        data.grassPlants = new List<GameData.GrassBladeData>();
        foreach(GrassBladeController gbc in activeGrassBlades)
        {
            data.grassPlants.Add(new GameData.GrassBladeData
            {
                location = gbc.transform.position,
                currentSize = gbc.currentSize
            });
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
