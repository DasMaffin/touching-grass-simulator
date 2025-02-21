using UnityEngine;

public class Settings
{

}

public class SettingsSave : MonoBehaviour
{
    #region Singleton

    private static SettingsSave _instance;

    public static SettingsSave Instance
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

    private void Awake()
    {
        Instance = this;
    }

    public void SaveMouseSensitivity(float sense)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", sense);
    }

    public Settings LoadCredits()
    {
        return null;
    }
}
