using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatUpdater : MonoBehaviour
{
    public TextMeshProUGUI moneyText;
    public Slider waterSlider;

    void Awake()
    {
        GameManager.Instance.player.OnMoneyChanged += UpdateMoneyUI;
        GameManager.Instance.player.OnAvailableWaterChanged += UpdateAvailableWaterUI;
    }

    void UpdateMoneyUI(float newValue)
    {
        moneyText.text = "Money: " + newValue.ToString("N2");
    }

    void UpdateAvailableWaterUI(float newValue)
    {
        waterSlider.value = newValue;
    }
}
