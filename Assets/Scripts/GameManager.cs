using UnityEngine;
using System;
using NUnit.Framework;
using System.Collections.Generic;

public class Player
{
    public event Action<int> OnGrassSeedsChanged;
    private int grassSeeds = 1;
    public int GrassSeeds
    {
        get { return grassSeeds; }
        set
        {
            grassSeeds = value;
            OnGrassSeedsChanged?.Invoke(grassSeeds); // Trigger event
        }
    }

    public event Action<int> OnGrassBladesChanged;
    private int grassBlades = 0;
    public int GrassBlades
    {
        get { return grassBlades; }
        set
        {
            grassBlades = value;
            OnGrassBladesChanged?.Invoke(grassBlades); // Trigger event
        }
    }

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

    public InteractiveTerrainTexture ITT;
    public Stack<GameObject> menuHistory = new Stack<GameObject>();
    public Player player;
    public GameObject[] grassSkin;
    public GameObject selectedGrassSkin;

    void Awake()
    {
        instance = this;
        player = new Player();
    }

    private void Start()
    {
        player.GrassSeeds = 1;
        player.GrassBlades = 0;
        player.Money = 0;
        player.AvailableWater = 100f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    internal void InstantiateGrass(Vector3 location)
    {
        Instantiate(selectedGrassSkin, location, Quaternion.identity);
    }
}
