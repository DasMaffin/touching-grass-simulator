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
    public class FlowerData
    {
        public FlowerType flower;
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

    [System.Serializable]
    public class Buildingdata
    {
        public Vector3 location;
    }

    [System.Serializable]
    public class WaterTankData : Buildingdata
    {
        public float fillAmount;
    }

    public float money;
    public float water;
    public float maxWater;
    public List<GrassBladeData> grassPlants;
    public List<FlowerData> flowerPlants;
    public List<WaterTankData> waterTanks;
    public List<InventorySlotData> inventorySlots;

    // These are the default values used when no savefile exists.
    public GameData()
    {
        this.money = 0;
        this.water = 10f;
        this.maxWater = 10f;
        flowerPlants = new List<FlowerData>();
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
        waterTanks = new List<WaterTankData>
        {
            new WaterTankData
            {
                location = new Vector3(497,0,502),
                fillAmount = 0f
            }
        };
    }
}
