using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class GrassSkinSelector : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int myIndex;
    [SerializeField] private GameObject border;

    private static event Action<int> OnSelectedChanged;
    private static int selectedSkin = int.MinValue;

    private void Start()
    {
        OnSelectedChanged += OnSelectedChangedHandler;
        if(selectedSkin == int.MinValue)
        {
            selectedSkin = PlayerPrefs.GetInt("SelectedSkin", 0);
        }
        if(myIndex == selectedSkin)
        {
            GameManager.Instance.selectedGrassSkin = myIndex;
            border.SetActive(true);
        }
        else
        {
            border.SetActive(false);
        }
    }

    private void OnSelectedChangedHandler(int index)
    {
        if(index == myIndex)
        {
            border.SetActive(true);
        }
        else
        {
            border.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.Instance.selectedGrassSkin = myIndex;
        OnSelectedChanged?.Invoke(myIndex);
        SettingsSave.Instance.SaveSelectedSkin(myIndex);
    }
}
