using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
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

    private Transform OGParent;
    private bool isDragging = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        OGParent = this.transform.parent;
        this.transform.SetParent(InventoryContainer.transform, false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        this.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        GameObject hoveredElement = GetHoveredUIElement();

        if (hoveredElement != null && hoveredElement != OGParent.gameObject && hoveredElement.layer == LayerMask.NameToLayer("InventorySlot")) 
        {
            this.transform.SetParent(hoveredElement.transform, false);
            hoveredElement.GetComponent<InventorySlotController>().isFull = true;
            OGParent.GetComponent<InventorySlotController>().isFull = false;
        }
        else
        {
            this.transform.SetParent(OGParent, false);
        }

        foreach(Transform sibling in GetSiblings(transform))
        {
            sibling.transform.SetParent(OGParent, false);
            OGParent.GetComponent<InventorySlotController>().isFull = true;
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

    private void Update()
    {
        if(isDragging)
        {
            this.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        }
    }
}
