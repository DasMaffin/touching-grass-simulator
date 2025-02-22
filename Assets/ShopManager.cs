using System;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    #region Singleton

    private static ShopManager _instance;

    public static ShopManager Instance
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

    public Action BuyAllItems;

    private void Awake()
    {
        Instance = this;
    }

    public void BuyAll()
    {
        BuyAllItems?.Invoke();
    }
}
