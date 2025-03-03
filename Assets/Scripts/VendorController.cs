using UnityEngine;
using UnityEngine.UI;

public class VendorController : Interactible
{
    public bool GrassBuyer = true;
    public override void OnHoverEnter()
    {
        outline.enabled = true;
        base.OnHoverEnter();
    }

    public override void OnHoverExit()
    {
        outline.enabled = false;
        base.OnHoverExit();
    }

    public override void onInteract()
    {
        if(GrassBuyer)
        {
            if(InventoryManager.Instance.GetItemCount(Item.GrassBlades) <= 0)
            {
                return;
            }
            GameManager.Instance.player.Money += 1.15f * InventoryManager.Instance.GetItemCount(Item.GrassBlades);
            InventoryManager.Instance.RemoveAllItems(Item.GrassBlades);
        }
        else
        {
            UIController.Instance.Shop.SetActive(true);
            GameManager.Instance.menuHistory.Push(UIController.Instance.Shop);
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
