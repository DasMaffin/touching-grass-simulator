using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopEntryController : MonoBehaviour
{
    private static readonly List<ShopEntryController> instances = new();

    public static IEnumerable<ShopEntryController> Instances => instances;

    private void OnEnable() => instances.Add(this);
    private void OnDisable() => instances.Remove(this);

    private float availableMoney 
    { 
        get
        {
            return GameManager.Instance.player.Money - currentTotalPrice;
        } 
    }

    private float currentTotalPrice 
    {
        get
        {
            return Instances.Sum(i =>
            {
                if(i != this)
                {
                    return i.totalAmount * i.price;
                }
                else return 0;
            });
        }
    }

    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private float price;
    [SerializeField] private Item shopItem;

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
    private int maxAmount { get => Mathf.FloorToInt(availableMoney / price); }

    private void Awake()
    {
        ShopManager.Instance.BuyAllItems += Buy;
        inputField.text = "0";
    }

    public void Buy()
    {
        InventoryManager.Instance.AddItem(shopItem, totalAmount);

        GameManager.Instance.player.Money -= totalPrice;
        inputField.text = "0";
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
        else if(availableMoney > price)
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
        if(int.TryParse(newInput, out int res) && res * price > availableMoney)
        {
            inputField.text = maxAmount.ToString();
        }
    }
}
