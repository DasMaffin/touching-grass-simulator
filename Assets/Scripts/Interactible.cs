using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Interactible : MonoBehaviour
{
    protected Renderer myRenderer;
    protected Outline outline;

    protected void Awake()
    {
        outline = GetComponent<Outline>();
    }
    public virtual void onInteract() { }
    public virtual void OnHoverEnter()
    {
        if(PlayerController.Instance != null)
            PlayerController.Instance.canUseWateringCan = false;
    }
    public virtual void OnHoverExit()
    {
        if(PlayerController.Instance != null)
            PlayerController.Instance.canUseWateringCan = true;
    }
}
