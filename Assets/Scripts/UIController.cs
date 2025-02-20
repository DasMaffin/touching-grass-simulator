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
                GameManager.Instance.menuHistory[GameManager.Instance.menuHistory.Count - 1].SetActive(false);
                GameManager.Instance.menuHistory.RemoveAt(GameManager.Instance.menuHistory.Count - 1);
                if(GameManager.Instance.menuHistory.Count == 0)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
            else
            {
                Settings.SetActive(true);
                GameManager.Instance.menuHistory.Add(Settings);
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }
}
