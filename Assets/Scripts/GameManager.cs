using UnityEngine;
using System;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEngine.Windows;
using Unity.Collections;
using Unity.Jobs;
using Unity.VisualScripting;

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

[System.Serializable]
public class GrassSkin
{
    public GameObject skin;
    public GameObject preview;
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

    public event Action<float> OnBuildChanged;

    public InteractiveTerrainTexture ITT;
    public GrassSkin[] grassSkins;
    public Flower[] flowers;
    public List<GrassBladeController> activeGrassBlades = new List<GrassBladeController>();
    public List<FlowerController> activeFlowers = new List<FlowerController>();
    public BuildInfo BuildInfo;

    [HideInInspector] public Player player;
    [HideInInspector] public Dictionary<Item, Sprite> itemSpriteMap;
    [HideInInspector] public List<ObjectPool<GameObject>> grassPools = new List<ObjectPool<GameObject>>();
    [HideInInspector] public List<ObjectPool<GameObject>> flowerPools = new List<ObjectPool<GameObject>>();
    [HideInInspector] public Stack<GameObject> menuHistory = new Stack<GameObject>();
    [HideInInspector] public int selectedGrassSkin;
    [HideInInspector] public Dictionary<GameObject, GrassBladeController> grassCache = new Dictionary<GameObject, GrassBladeController>();
    [HideInInspector] public Dictionary<GameObject, FlowerController> flowerCache = new Dictionary<GameObject, FlowerController>();

    [SerializeField] private Sprite noneSprite;
    [SerializeField] private Sprite seedsSprite;
    [SerializeField] private Sprite bladesSprite;
    [SerializeField] private Sprite wateringCanSprite;
    [SerializeField] private Sprite daisySeedsSprite;

    void Awake()
    {
        Instance = this;

        player = new Player();
        itemSpriteMap = new Dictionary<Item, Sprite>
        {
            { Item.None, noneSprite },
            { Item.GrassSeeds, seedsSprite },
            { Item.GrassBlades, bladesSprite },
            { Item.WateringCan, wateringCanSprite },
            { Item.DaisySeeds, daisySeedsSprite }
        };

        foreach(GrassSkin skin in grassSkins)
        {
            skin.preview = Instantiate(skin.preview);
        }
        foreach(Flower skin in flowers)
        {
            skin.preview = Instantiate(skin.preview);
        }
    }

    private void Start()
    {
        player.AvailableWater = 100f;

        OnBuildChanged?.Invoke(BuildInfo.buildNumber);

        #region SFXVolume

        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", .5f);
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

        foreach(var pool in flowerPools)
        {
            pool.Clear(); // Ensure Unity's ObjectPool clears its stored instances
        }
        flowerPools.Clear();

        for(int i = 0; i < grassSkins.Length; i++)
        {
            int index = i;
            grassPools.Add(new ObjectPool<GameObject>(
                () =>
                {
                    GameObject go = Instantiate(grassSkins[index].skin);
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
                    gbc.affectingFlowers = new List<FlowerController>();
                    gbc.OnHoverExit();
                    gbc.growObject.transform.localScale = new Vector3(gbc.currentSize, gbc.currentSize, gbc.currentSize);
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
                GameObject go = Instantiate(grassSkins[index].skin);
                GrassBladeController gbc = go.GetComponent<GrassBladeController>();
                gbc.selectedGrassSkin = index;
                grassCache[go] = gbc;
                grassPools[i].Release(go);
            }
        }

        for(int i = 0; i < flowers.Length; i++)
        {
            int index = i;
            flowerPools.Add(new ObjectPool<GameObject>(
                () =>
                {
                    GameObject go = Instantiate(flowers[index].prefab);
                    FlowerController fc = go.GetComponent<FlowerController>();
                    fc.poolIndex = index;
                    flowerCache[go] = fc;
                    return go;
                },
                go =>
                {
                    activeFlowers.Add(flowerCache[go]);
                    go.SetActive(true);
                },
                go =>
                {
                    if(!flowerCache.TryGetValue(go, out FlowerController fc))
                    {
                        fc = go.GetComponent<FlowerController>();
                        flowerCache.Add(go, fc);
                    }
                    fc.watered = false;
                    fc.currentSize = 0.01f;
                    fc.OnHoverExit();
                    fc.growObject.transform.localScale = new Vector3(fc.currentSize, fc.currentSize, fc.currentSize);
                    activeFlowers.Remove(flowerCache[go]);
                    go.SetActive(false);
                },
                go => Destroy(go),
                false,
                input,
                input * 10
            ));

            for(int j = 0; j < input; j++)
            {
                GameObject go = Instantiate(flowers[index].prefab);
                FlowerController fc = go.GetComponent<FlowerController>();
                fc.poolIndex = index;
                flowerCache[go] = fc;
                flowerPools[i].Release(go);
            }
        }
    }

    private void Update()
    {
        player.UseWateringCan();
    }

    internal GameObject InstantiatePlant(Vector3 location, Item item)
    {
        switch(item)
        {
            case Item.GrassSeeds:            
                GameObject grass = grassPools[selectedGrassSkin].Get();
                grass.transform.position = location;
                grass.transform.rotation = Quaternion.identity;
                return grass;
            case Item.DaisySeeds:
                GameObject daisy = flowerPools[0].Get();
                daisy.transform.position = location;
                daisy.transform.rotation = Quaternion.identity;
                return daisy;
            default:
                break;
        }
        throw new Exception();
    }

    public void LoadData(GameData data)
    {
        this.player.Money = data.money;
        this.player.AvailableWater = 100f; // TODO data.water;
        foreach(GameData.GrassBladeData gbd in data.grassPlants)
        {
            GrassBladeController gbc = InstantiatePlant(gbd.location, Item.GrassSeeds).GetComponent<GrassBladeController>();
            gbc.currentSize = gbd.currentSize;
        }

        foreach(GameData.FlowerData fd in data.flowerPlants)
        {
            switch(fd.flower)
            {
                case FlowerType.Daisy:                
                    FlowerController fc = InstantiatePlant(fd.location, Item.DaisySeeds).GetComponent<FlowerController>();
                    fc.currentSize = fd.currentSize;
                    break;
                default:
                    break;
            }
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
        data.flowerPlants = new List<GameData.FlowerData>();
        foreach(FlowerController fc in activeFlowers)
        {
            data.flowerPlants.Add(new GameData.FlowerData
            {
                location = fc.transform.position,
                currentSize = fc.currentSize,
                flower = fc.Flower
            });
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
