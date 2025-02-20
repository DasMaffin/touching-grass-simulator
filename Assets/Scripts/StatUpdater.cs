using TMPro;
using UnityEngine;

public class StatUpdater : MonoBehaviour
{
    public TextMeshProUGUI grassSeedsText;
    public TextMeshProUGUI grassBladesText;
    public TextMeshProUGUI moneyText;

    void Awake()
    {
        GameManager.Instance.OnGrassSeedsChanged += UpdateGrassSeedsUI;
        GameManager.Instance.OnGrassBladesChanged += UpdateGrassBladesUI;
        GameManager.Instance.OnMoneyChanged += UpdateMoneyUI;
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

}
