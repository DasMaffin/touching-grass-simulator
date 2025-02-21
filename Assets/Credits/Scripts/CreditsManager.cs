using UnityEngine;

public class CreditsManager : MonoBehaviour
{
    #region Singleton

    private static CreditsManager _instance;

    public static CreditsManager Instance
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

    private int clickedCredits = 0;

    public int ClickedCredits 
    {
        get => clickedCredits;
        set
        {
            clickedCredits = value;
            if(clickedCredits == MinimumCreditsToClick) creditController.CanBeMinimized = true;
        }
    }

    public int MinimumCreditsToClick = 3;
    public MinimizeUIController creditController;


    void Awake()
    {
        Instance = this;
    }
}
