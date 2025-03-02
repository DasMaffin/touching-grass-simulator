using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static InventoryItemController SelectedInstance;

    private static GameObject inventoryContainer;
    private static GameObject InventoryContainer
    {
        get
        {
            if(inventoryContainer == null)
            {
                inventoryContainer = GameObject.FindGameObjectWithTag("InventoryContainer");
            }
            return inventoryContainer;
        }
    }

    public InventoryItem item;

    private Transform OGParent;
    private bool isDragging = false;
    private bool shiftClicking = false;

    private void Start()
    {
        this.GetComponent<RectTransform>().sizeDelta = this.transform.parent.GetComponentInParent<RectTransform>().sizeDelta * 0.9f; // TODO Refactor the GetComponents out.
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(isDragging) return;
        OGParent = this.transform.parent;
        if(shiftClicking)
        {
            while(this.item.Owned != 0)
            {
                InventoryItem iItem = InventoryManager.Instance.GetNonFullStack(this.item.Item, !OGParent.GetComponent<InventorySlotController>().isHotBar);
                if(iItem != null)
                {
                    if(iItem.Owned + this.item.Owned > iItem.maxStackSize)
                    {
                        int diff = iItem.maxStackSize - iItem.Owned;
                        iItem.Owned = iItem.maxStackSize;
                        InventoryManager.Instance.RemoveItem(this.item.Item, diff, this.item);
                    }
                    else
                    {
                        iItem.Owned += this.item.Owned;
                        InventoryManager.Instance.RemoveItem(this.item.Item, this.item.Owned, this.item);
                        return;
                    }
                }
                else
                {
                    break;
                }
            }
            InventorySlotController isc = InventoryManager.Instance.GetFreeSlot(!OGParent.GetComponent<InventorySlotController>().isHotBar);
            OGParent.GetComponent<InventorySlotController>().isFull = false;
            OGParent.GetComponent<InventorySlotController>().ItemInSlot = null;
            isc.isFull = true;
            isc.ItemInSlot = item;
            item.slot = isc;
            this.transform.SetParent(isc.transform, false);

            if(isc.slotId == InventoryBarManager.Instance.ActiveSlot + 40)
            {
                isc.ItemInSlot.Select();
            }
        }
        else
        {
            isDragging = true;
            this.transform.SetParent(InventoryContainer.transform, false);
            SelectedInstance = this;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(!isDragging) return;
        SelectedInstance = null;
        isDragging = false;
        this.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        GameObject hoveredElement = GetHoveredUIElement("InventorySlot");

        if (hoveredElement != null && hoveredElement != OGParent.gameObject && hoveredElement.layer == LayerMask.NameToLayer("InventorySlot"))
        {
            InventorySlotController isc = hoveredElement.GetComponent<InventorySlotController>();

            if(isc != null && isc.ItemInSlot != null && isc.ItemInSlot.Item == this.item.Item)
            {
                if(isc.ItemInSlot.Owned + item.Owned > item.maxStackSize)
                {
                    InventoryManager.Instance.RemoveItem(item.Item, isc.ItemInSlot.maxStackSize - isc.ItemInSlot.Owned, item);
                    isc.ItemInSlot.Owned = isc.ItemInSlot.maxStackSize;
                }
                else
                {
                    isc.ItemInSlot.Owned += item.Owned;
                    InventoryManager.Instance.RemoveItem(item.Item, item.Owned, item);
                }
                this.transform.SetParent(OGParent, false);
                return;
            }
            else
            {
                if(OGParent.GetComponent<InventorySlotController>().isHotBar && OGParent.GetComponent<InventorySlotController>().slotId == InventoryBarManager.Instance.ActiveSlot + 40)
                {
                    item.Deselect();
                }
                this.transform.SetParent(hoveredElement.transform, false);
                if(hoveredElement.GetComponent<InventorySlotController>().isHotBar && hoveredElement.GetComponent<InventorySlotController>().slotId == InventoryBarManager.Instance.ActiveSlot + 40)
                {
                    item.Select();
                }

                isc.isFull = true;
                isc.ItemInSlot = item;
                item.slot = isc;
                OGParent.GetComponent<InventorySlotController>().isFull = false;
                OGParent.GetComponent<InventorySlotController>().ItemInSlot = null;
            }
        }
        else
        {
            this.transform.SetParent(OGParent, false);
        }
        
        foreach(Transform sibling in GetSiblings(transform))
        {
            if(sibling.tag == "IgnoreInventory")
            {
                continue;
            }
            if(sibling.GetComponentInParent<InventorySlotController>().isHotBar && sibling.GetComponentInParent<InventorySlotController>().slotId == InventoryBarManager.Instance.ActiveSlot + 40)
            {
                sibling.GetComponent<InventoryItemController>().item.Deselect();
            }
            sibling.transform.SetParent(OGParent, false);
            OGParent.GetComponent<InventorySlotController>().isFull = true;
            OGParent.GetComponent<InventorySlotController>().ItemInSlot = sibling.GetComponent<InventoryItemController>().item;
            sibling.GetComponent<InventoryItemController>().item.slot = OGParent.GetComponent<InventorySlotController>();
            if(OGParent.GetComponent<InventorySlotController>().isHotBar && OGParent.GetComponent<InventorySlotController>().slotId == InventoryBarManager.Instance.ActiveSlot + 40)
            {
                sibling.GetComponent<InventoryItemController>().item.Select();
            }
        }
    }

    private List<Transform> GetSiblings(Transform go)
    {
        List<Transform> siblings = new List<Transform>();
        Transform parent = go.parent?.transform;
        if(parent != null)
        {
            for(int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if(child != go)
                {
                    siblings.Add(child);
                }
            }
        }
        return siblings;
    }

    public static GameObject GetHoveredUIElement(string layerName)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        foreach(RaycastResult result in results)
        {
            if(result.gameObject.layer == LayerMask.NameToLayer(layerName))
            {
                return result.gameObject;
            }
        }

        return null;
    }

    private void Update()
    {
        if(!isDragging)
            this.GetComponent<RectTransform>().sizeDelta = this.transform.parent.GetComponentInParent<RectTransform>().sizeDelta * 0.9f; // TODO Refactor the GetComponents out.
        if(isDragging)
        {
            this.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        }
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            shiftClicking = true;
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            shiftClicking = false;
        }
    }
}
