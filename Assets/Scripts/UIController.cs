using JetBrains.Annotations;
using System;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject Settings;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
   void Start()
    {
        Settings.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(GameManager.Instance.menuHistory.Count != 0)
            {
                GameManager.Instance.menuHistory.Pop().SetActive(false);
                if(GameManager.Instance.menuHistory.Count == 0)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
            else
            {
                Settings.SetActive(true);
                GameManager.Instance.menuHistory.Push(Settings);
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
    }
}
