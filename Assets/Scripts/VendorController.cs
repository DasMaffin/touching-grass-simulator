using UnityEngine;
using UnityEngine.UI;

public class VendorController : Interactible
{
    public bool GrassBuyer = true;
    public override void OnHoverEnter()
    {
        outline.enabled = true;
    }

    public override void OnHoverExit()
    {
        outline.enabled = false;
    }

    public override void onInteract()
    {
        if(GrassBuyer)
        {
            if(GameManager.Instance.GrassBlades <= 0)
            {
                return;
            }
            GameManager.Instance.Money += 1.5f * GameManager.Instance.GrassBlades;
            GameManager.Instance.GrassBlades = 0;
        }
        else
        {
            if(GameManager.Instance.Money < 1)
            {
                return;
            }
            GameManager.Instance.Money--;
            GameManager.Instance.GrassSeeds++;
        }
    }
}
