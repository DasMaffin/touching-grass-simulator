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

    private void Start()
    {
        this.GetComponent<RectTransform>().sizeDelta = this.transform.parent.GetComponentInParent<RectTransform>().sizeDelta * 0.9f; // TODO Refactor the GetComponents out.
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(isDragging) return;
        isDragging = true;
        OGParent = this.transform.parent;
        this.transform.SetParent(InventoryContainer.transform, false);
        SelectedInstance = this;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(!isDragging) return;
        SelectedInstance = null;
        isDragging = false;
        this.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        GameObject hoveredElement = GetHoveredUIElement();

        if (hoveredElement != null && hoveredElement != OGParent.gameObject && hoveredElement.layer == LayerMask.NameToLayer("InventorySlot"))
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

            InventorySlotController isc = hoveredElement.GetComponent<InventorySlotController>();
            isc.isFull = true;
            isc.ItemInSlot = item;
            item.slot = isc;
            OGParent.GetComponent<InventorySlotController>().isFull = false;
            OGParent.GetComponent<InventorySlotController>().ItemInSlot = null;
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

    public static GameObject GetHoveredUIElement()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        foreach(RaycastResult result in results)
        {
            if(result.gameObject.layer == LayerMask.NameToLayer("InventorySlot"))
            {
                return result.gameObject;
            }
        }

        return null;
    }

    private bool firstUpdate = true;
    private void Update()
    {
        if(firstUpdate)
        {
            this.GetComponent<RectTransform>().sizeDelta = this.transform.parent.GetComponentInParent<RectTransform>().sizeDelta * 0.9f; // TODO Refactor the GetComponents out.
            firstUpdate = false;
        }
        if(isDragging)
        {
            this.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        }
    }
}
