using UnityEngine;
using System;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.Pool;

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
            AvailableWater -= 1f * Time.deltaTime;
        }
    }
}

public class GameManager : MonoBehaviour
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

    [HideInInspector] public int selectedGrassSkin;

    [SerializeField] private Sprite noneSprite;
    [SerializeField] private Sprite seedsSprite;
    [SerializeField] private Sprite bladesSprite;

    private Dictionary<GameObject, GrassBladeController> grassCache = new Dictionary<GameObject, GrassBladeController>();

    void Awake()
    {
        instance = this;
        player = new Player();
        itemSpriteMap = new Dictionary<Item, Sprite>
        {
            { Item.None, noneSprite },
            { Item.GrassSeeds, seedsSprite },
            { Item.GrassBlades, bladesSprite }
        };
    }

    private void Start()
    {
        player.AvailableWater = 100f;
        player.Money = 50000f;

        for(int i = 0; i < grassSkins.Length; i++)
        {
            int index = i;
            grassPools.Add(new ObjectPool<GameObject>(
                () =>
                {
                    GameObject go = Instantiate(grassSkins[index]);
                    GrassBladeController gbc = go.GetComponent<GrassBladeController>();
                    grassCache[go] = gbc;
                    return go;
                },
                go => go.SetActive(true),
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
                    go.SetActive(false);
                },
                go => Destroy(go),
                false,
                10000,
                100000
            ));
        }
    }

    internal void InstantiateGrass(Vector3 location)
    {
        GameObject grass = grassPools[selectedGrassSkin].Get();
        grass.transform.position = location;
        grass.transform.rotation = Quaternion.identity;

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
