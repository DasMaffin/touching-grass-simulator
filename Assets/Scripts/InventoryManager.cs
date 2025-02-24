using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class InventoryItem
{
    private Item item;
    public Item Item
    {
        get => item;
        set
        {
            if(value == Item.WateringCan)
            {
                MaxStackSize = 1;
            }
            item = value;
        }
    }

    public Action<int, InventoryItem> OnOwnedChanged;
    private int owned;
    public int Owned
    {
        get => owned;
        set
        {
            owned = value;
            OnOwnedChanged?.Invoke(value, this);
        }
    }

    public Action<int, InventoryItem> OnMaxStacksizeChanged;
    public int maxStackSize = 1000;
    public int MaxStackSize 
    {
        get => maxStackSize;
        set
        {
            maxStackSize = value;
            OnMaxStacksizeChanged?.Invoke(value, this);
        }
    }
    public GameObject myItem;
    public InventorySlotController slot;

    public void Use()
    {
        if(GameManager.Instance.menuHistory.Count != 0) return;
        switch(Item)
        {
            case Item.GrassSeeds:
                LayerMask layersToCheck = (1 << 0) | (1 << 1) | (1 << 8); // 1 << layer
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(ray, out RaycastHit hit, 4, layersToCheck))
                {
                    GameManager.Instance.ITT.HandleGrassBlade(hit, this);
                }
                break;
            case Item.WateringCan:
                PlayerController.Instance.UseWateringCan();
                break;
            case Item.GrassBlades:
            default:
                Debug.Log("This item cant be used.");
                break;
        }
    }

    public void EndUse()
    {
        switch(Item)
        {
            case Item.WateringCan:
                PlayerController.Instance.StopUsingWateringCan();
                break;
            case Item.GrassBlades:
            case Item.GrassSeeds:
            default:
                break;
        }
    }

    public void Select()
    {
        switch(Item)
        {
            case Item.WateringCan:
                PlayerController.Instance.SelectWateringCan();
                break;
            case Item.GrassBlades:
            case Item.GrassSeeds:
            default:
                Debug.Log($"Item \"{Item}\" Selected!");
                break;
        }
    }

    public void Deselect()
    {
        switch(Item)
        {
            case Item.WateringCan:
                PlayerController.Instance.DeselectWateringCan();
                break;
            case Item.GrassBlades:
            case Item.GrassSeeds:
            default:
                Debug.Log($"Item \"{Item}\" deselected!");
                break;
        }
    }
}

public class InventoryManager : MonoBehaviour
{
    #region Singleton

    private static InventoryManager instance;
    public static InventoryManager Instance
    {
        get { return instance; }
        set
        {
            if(instance != null)
            {
                Destroy(value.gameObject);
                return;
            }
            instance = value;
        }
    }

    #endregion

    [SerializeField] private GameObject inventoryItemPrefab;

    private ObservableCollection<InventoryItem> items = new ObservableCollection<InventoryItem>();

    [SerializeField] private List<InventorySlotController> inventorySlots = new List<InventorySlotController>();

    private Dictionary<Item, Type> itemToType = new Dictionary<Item, Type>();

    private void Awake()
    {
        Instance = this;

        inventorySlots.AddRange(GameObject.FindGameObjectsWithTag("InventorySlot")
            .Select(go => go.GetComponent<InventorySlotController>())
            .OrderBy(slot => slot.slotId)
            .ToList());

        items.CollectionChanged += (sender, e) =>
        {
            switch(e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    InventorySlotController slot = inventorySlots.FirstOrDefault(s => !s.isFull);
                    if(slot == null)
                    {
                        Debug.LogWarning("No free slots in inventory");
                        return; 
                    }
                    InventoryItem newItem = (e.NewItems[0] as InventoryItem);
                    newItem.OnOwnedChanged += InventoryItemOwnedChanged;
                    newItem.OnMaxStacksizeChanged += InventoryItemStacksizeChanged;
                    newItem.myItem = Instantiate(inventoryItemPrefab, slot.transform); // TODO create a pool of items
                    newItem.myItem.GetComponent<Image>().sprite = GameManager.Instance.itemSpriteMap[newItem.Item];
                    newItem.slot = slot;
                    slot.isFull = true;
                    slot.ItemInSlot = newItem;

                    newItem.myItem.GetComponent<InventoryItemController>().item = newItem;
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    InventoryItem oldItem = (e.OldItems[0] as InventoryItem);
                    oldItem.OnOwnedChanged -= InventoryItemOwnedChanged;
                    oldItem.OnMaxStacksizeChanged -= InventoryItemStacksizeChanged;
                    oldItem.slot.isFull = false;
                    oldItem.slot.ItemInSlot = null;
                    Destroy(oldItem.myItem); // TODO Release to pool
                    break;
                default:
                    Debug.LogWarning($"No handling for action {e.Action} on InventoryManager.items");
                    break;
            }
        };
    }

    private void InventoryItemStacksizeChanged(int stacksize, InventoryItem item)
    {
        if(stacksize <= 1)
        {
            item.myItem.GetComponentInChildren<TMP_Text>().text = "";
        }
    }

    private void InventoryItemOwnedChanged(int owned, InventoryItem item)
    {
        item.myItem.GetComponentInChildren<TMP_Text>().text = owned.ToString();
    }

    public void AddItem(Item item, int amount)
    {
        InventoryItem inventoryItem = items.ToList().Find(i => i.Item == item && i.Owned < i.MaxStackSize);
        if(inventoryItem == null) // If there is no item of this in the inventory or only full stacks
        {
            inventoryItem = new InventoryItem { Item = item };
            items.Add(inventoryItem);
        }
        inventoryItem.Owned += amount;
        while(inventoryItem.Owned > inventoryItem.MaxStackSize)
        {
            InventoryItem newStack = new InventoryItem { Item = item };
            items.Add(newStack);
            newStack.Owned = Math.Clamp(inventoryItem.Owned - inventoryItem.MaxStackSize, 0, inventoryItem.MaxStackSize);
            inventoryItem.Owned -= newStack.Owned;
        }
    }

    public void RemoveItem(Item item, int amount, InventoryItem inventoryItem)
    {
        if(inventoryItem == null)
        {
            throw new Exception("Item not found in inventory");
        }
        
        inventoryItem.Owned -= amount;
        if(inventoryItem.Owned == 0) items.Remove(inventoryItem);
    }

    public void RemoveItem(Item item, int amount)
    {
        while(amount != 0)
        {
            InventoryItem inventoryItem = items.ToList().Find(i => i.Item == item);
            if(inventoryItem == null)
            {
                throw new Exception("Item not found in inventory");
            }

            if(amount >= inventoryItem.Owned)
            {
                amount -= inventoryItem.Owned;
                items.Remove(inventoryItem);
            }
            else
            {
                inventoryItem.Owned -= amount;
                amount = 0;
            }
        }
    }

    public int GetItemCount(Item item)
    {
        List<InventoryItem> inventoryItems = items.ToList().FindAll(i => i.Item == item);
        if(inventoryItems == null || inventoryItems.Count == 0)
        {
            return 0;
        }
        return inventoryItems.Sum(i => i.Owned);
    }

    internal void RemoveAllItems(Item item)
    {
        List<InventoryItem> inventoryItems = items.ToList().FindAll(i => i.Item == item);

        foreach(InventoryItem iItem in inventoryItems)
        {
            items.Remove(iItem);
        }
    }

    public InventoryItem GetItemInSlot(int slot)
    {
        InventorySlotController slotController = inventorySlots[slot];

        return slotController.ItemInSlot;
    }
}
