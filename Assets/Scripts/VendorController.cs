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
            if(GameManager.Instance.player.GrassBlades <= 0)
            {
                return;
            }
            GameManager.Instance.player.Money += 1.5f * GameManager.Instance.player.GrassBlades;
            GameManager.Instance.player.GrassBlades = 0;
        }
        else
        {
            if(GameManager.Instance.player.Money < 1)
            {
                return;
            }
            GameManager.Instance.player.Money--;
            GameManager.Instance.player.GrassSeeds++;
        }
    }
}
