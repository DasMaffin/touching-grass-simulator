using UnityEngine;
using System;
using NUnit.Framework;
using System.Collections.Generic;

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

    public InteractiveTerrainTexture ITT;

    public Stack<GameObject> menuHistory = new Stack<GameObject>();

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GrassSeeds = 1;
        GrassBlades = 0;
        Money = 0;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
