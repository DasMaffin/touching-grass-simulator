using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ShopItem
{
    None = 0,
    GrassSeeds = 1,
    Water = 2
}

public class ShopEntryController : MonoBehaviour
{

    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private float price;
    [SerializeField] private ShopItem shopItem;

    private int totalAmount
    {
        get
        {
            if(int.TryParse(inputField.text, out int result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }
    }
    private int totalPrice { get => totalAmount * (int)price; }
    private int maxAmount { get => Mathf.FloorToInt(GameManager.Instance.player.Money / price); }

    private void Awake()
    {
        ShopManager.Instance.BuyAllItems += Buy;
    }

    public void Buy()
    {
        switch(shopItem)
        {
            case ShopItem.GrassSeeds:
                GameManager.Instance.player.GrassSeeds += totalAmount;
                break;
            case ShopItem.Water:
            case ShopItem.None:
                break;
        }
        GameManager.Instance.player.Money -= totalPrice;
    }

    public void SelectMax()
    {
        inputField.text = maxAmount.ToString();
    }

    public void OneMore()
    {
        if(inputField.text == "")
        {
            inputField.text = "0";
        }
        else if(totalAmount < maxAmount)
        {
            inputField.text = (int.Parse(inputField.text) + 1).ToString();
        }
    }

    public void OneLess()
    {
        if(inputField.text == "" || inputField.text == "0")
        {
            inputField.text = "0";
        }
        else
        {
            inputField.text = (int.Parse(inputField.text) - 1).ToString();
        }
    }

    public void OnInputChanged(string newInput)
    {
        if(int.TryParse(newInput, out int res) && res > maxAmount)
        {
            inputField.text = maxAmount.ToString();
        }
    }
}
