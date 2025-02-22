using JetBrains.Annotations;
using System;
using UnityEngine;

public class UIController : MonoBehaviour
{
    #region Singleton

    private static UIController _instance;

    public static UIController Instance
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

    public GameObject Settings;
    public GameObject Shop;
    public GameObject Inventory;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Settings.SetActive(false);
        Shop.SetActive(false);
        Inventory.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(GameManager.Instance.menuHistory.Count != 0)
            {

                CloseSettings(Settings);
            }
            else
            {
                Settings.SetActive(true);
                GameManager.Instance.menuHistory.Push(Settings);
                Cursor.lockState = CursorLockMode.None;
            }
        }
        else if(Input.GetKeyDown(KeyCode.Tab))
        {
            if(GameManager.Instance.menuHistory.Count != 0)
            {
                CloseSettings(Inventory);
            }
            else
            {
                Inventory.SetActive(true);
                GameManager.Instance.menuHistory.Push(Inventory);
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    public void CloseSettings(GameObject closeObject)
    {
        while(true)
        {
            GameObject go = GameManager.Instance.menuHistory.Pop();
            go.SetActive(false);
            if(go == closeObject)
            {
                break;
            }
        }
        Cursor.lockState = CursorLockMode.Locked;
    }
}
