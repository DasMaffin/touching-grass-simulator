using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    [System.Serializable]
    public class GrassBladeData
    {
        public Vector3 location;
        public float currentSize;
    }

    [System.Serializable]
    public class  InventorySlotData
    {
        public int slotId;
        public Item InventoryItemItem;
        public int InventoryItemOwned;
    }

    public float money;
    public List<GrassBladeData> grassPlants;
    public List<InventorySlotData> inventorySlots;

    // These are the default values used when no savefile exists.
    public GameData()
    {
        this.money = 0;
        grassPlants = new List<GrassBladeData>();
        grassPlants.Add(new GrassBladeData
        {
            location = new Vector3(500, 0, 503),
            currentSize = 1f
        });
        inventorySlots = new List<InventorySlotData>();
        inventorySlots.Add(new InventorySlotData
        {
            slotId = 0,
            InventoryItemItem= Item.WateringCan,
            InventoryItemOwned = 1
        });
    }
}
