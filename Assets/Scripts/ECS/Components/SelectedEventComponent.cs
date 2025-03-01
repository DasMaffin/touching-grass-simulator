using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public struct SelectedEventComponent : IComponentData, IEnableableComponent
{
    public bool onSelected;
    public bool onDeselected;
    public bool onInteract;
}
