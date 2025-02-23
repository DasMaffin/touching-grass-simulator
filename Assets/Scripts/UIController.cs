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

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            HandleMenuInput(Settings);
        }
        else if(Input.GetKeyDown(KeyCode.Tab))
        {
            HandleMenuInput(Inventory);
        }
    }

    void HandleMenuInput(GameObject menuToOpen)
    {
        if(GameManager.Instance.menuHistory.Count != 0)
        {
            CloseMenu(GameManager.Instance.menuHistory.Peek());
        }
        else
        {
            menuToOpen.SetActive(true);
            GameManager.Instance.menuHistory.Push(menuToOpen);
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void CloseMenu(GameObject closeObject)
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
