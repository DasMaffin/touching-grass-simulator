using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class InventoryItem
{
    public Item item;

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
    public int maxStackSize = 1000;
    public GameObject myItem;
    public InventorySlotController slot;
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
                    newItem.myItem = Instantiate(inventoryItemPrefab, slot.transform); // TODO create a pool of items
                    newItem.myItem.GetComponent<Image>().sprite = GameManager.Instance.itemSpriteMap[newItem.item];
                    newItem.slot = slot;
                    slot.isFull = true;
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    InventoryItem oldItem = (e.OldItems[0] as InventoryItem);
                    oldItem.OnOwnedChanged -= InventoryItemOwnedChanged;
                    oldItem.slot.isFull = false;
                    Destroy(oldItem.myItem); // TODO Release to pool
                    break;
                default:
                    Debug.LogWarning($"No handling for action {e.Action} on InventoryManager.items");
                    break;
            }
        };
    }

    private void InventoryItemOwnedChanged(int owned, InventoryItem item)
    {
        item.myItem.GetComponentInChildren<TMP_Text>().text = owned.ToString();
    }

    public void AddItem(Item item, int amount)
    {
        InventoryItem inventoryItem = items.ToList().Find(i => i.item == item && i.Owned < i.maxStackSize);
        if(inventoryItem == null) // If there is no item of this in the inventory or only full stacks
        {
            inventoryItem = new InventoryItem { item = item };
            items.Add(inventoryItem);
        }
        inventoryItem.Owned += amount;
        while(inventoryItem.Owned > inventoryItem.maxStackSize)
        {
            InventoryItem newStack = new InventoryItem { item = item };
            items.Add(newStack);
            newStack.Owned = Math.Clamp(inventoryItem.Owned - inventoryItem.maxStackSize, 0, inventoryItem.maxStackSize);
            inventoryItem.Owned -= newStack.Owned;
        }
    }

    public void RemoveItem(Item item, int amount)
    {
        while(amount != 0)
        {
            InventoryItem inventoryItem = items.ToList().Find(i => i.item == item);
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
        List<InventoryItem> inventoryItems = items.ToList().FindAll(i => i.item == item);
        if(inventoryItems == null || inventoryItems.Count == 0)
        {
            return 0;
        }
        return inventoryItems.Sum(i => i.Owned);
    }

    internal void RemoveAllItems(Item item)
    {
        List<InventoryItem> inventoryItems = items.ToList().FindAll(i => i.item == item);

        foreach(InventoryItem iItem in inventoryItems)
        {
            items.Remove(iItem);
        }
    }
}
