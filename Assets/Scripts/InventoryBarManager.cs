using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryBarManager : MonoBehaviour
{
    #region Singleton

    private static InventoryBarManager instance;
    public static InventoryBarManager Instance
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
    private List<InventoryBarSlot> Slots = new List<InventoryBarSlot>();
    private int activeSlot = 0;
    public int ActiveSlot 
    {
        get => activeSlot;
        set
        {
            if(value < 0)
            {
                ActiveSlot = Slots.Count - 1;
                return;
            }
            else if(value >= Slots.Count)
            {
                ActiveSlot = 0;
                return;
            }
            InventoryManager.Instance.GetItemInSlot(ActiveSlot + 40)?.Deselect();
            Slots[activeSlot].SlotDecoration.SetActive(false);
            activeSlot = value;
            InventoryManager.Instance.GetItemInSlot(ActiveSlot + 40)?.Select();
            Slots[activeSlot].SlotDecoration.SetActive(true);
        }
    }

    private class InventoryBarSlot
    {
        public GameObject Slot;
        public GameObject SlotDecoration;
        public InventorySlotController SlotController;
        public InventoryItem Item;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach(Transform child in GetChildrenRecursive(this.transform, 1))
        {
            InventoryBarSlot ibs = new InventoryBarSlot 
            {
                Slot = child.gameObject,
                SlotDecoration = child.gameObject.GetComponentsInChildren(typeof(Component)).Where(r => r.tag == "InventoryDecoration").ToArray()[0].gameObject,
                SlotController = child.GetComponent<InventorySlotController>()
            };
            Slots.Add(ibs);
        }
        Slots = Slots.OrderBy(slot => slot.SlotController.slotId).ToList();

        foreach(InventoryBarSlot go in Slots)
        {
            go.SlotDecoration.SetActive(false);
        }
        ActiveSlot = 0;
    }

    private void Update()
    {
        if(GameManager.Instance.menuHistory.Count > 0) return;
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if(scroll < 0)
        {
            ActiveSlot += 1;
        }
        else if(scroll > 0)
        {
            ActiveSlot -= 1;
        }

        for(int i = 0; i <= 9; i++)
        {
            if(Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha0 + i)))
            {
                ActiveSlot = i - 1;
                break;
            }
        }

        if(Input.GetKeyDown(KeyCode.Mouse0) && GameManager.Instance.menuHistory.Count == 0)
        {
            InventoryManager.Instance.GetItemInSlot(ActiveSlot + 40)?.Use();
        }
        if(Input.GetKeyUp(KeyCode.Mouse0) && GameManager.Instance.menuHistory.Count == 0)
        {
            InventoryManager.Instance.GetItemInSlot(ActiveSlot + 40)?.EndUse();
        }
    }

    public List<Transform> GetChildrenRecursive(Transform parent, int depth)
    {
        List<Transform> children = new List<Transform>();
        GetChildrenRecursiveInternal(parent, depth, children);
        return children;
    }

    private void GetChildrenRecursiveInternal(Transform parent, int depth, List<Transform> children)
    {
        if(depth <= 0) return;

        foreach(Transform child in parent)
        {
            children.Add(child);
            GetChildrenRecursiveInternal(child, depth - 1, children);
        }
    }
}
