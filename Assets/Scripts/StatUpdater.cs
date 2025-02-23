using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatUpdater : MonoBehaviour
{
    public TextMeshProUGUI grassSeedsText;
    public TextMeshProUGUI grassBladesText;
    public TextMeshProUGUI moneyText;
    public Slider waterSlider;

    void Awake()
    {
        GameManager.Instance.player.OnMoneyChanged += UpdateMoneyUI;
        GameManager.Instance.player.OnAvailableWaterChanged += UpdateAvailableWaterUI;
    }

    void UpdateGrassSeedsUI(int newValue)
    {
        grassSeedsText.text = "Grass seeds: " + newValue.ToString();
    }

    void UpdateGrassBladesUI(int newValue)
    {
        grassBladesText.text = "Grass blades: " + newValue.ToString();
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
