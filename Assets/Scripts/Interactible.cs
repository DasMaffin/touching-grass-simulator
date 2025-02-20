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
    public abstract void onInteract();
    public abstract void OnHoverEnter();
    public abstract void OnHoverExit();
}
