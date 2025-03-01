using UnityEngine;
using Unity.Entities;
using UnityEngine.Rendering;

public class SelectedEventAuthoring : MonoBehaviour
{
    public class Baker : Baker<SelectedEventAuthoring> 
    {
        public override void Bake(SelectedEventAuthoring authoring) 
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SelectedEventComponent
            {
                onSelected = false,
                onDeselected = false,
                onInteract = false
            });
            SetComponentEnabled<SelectedEventComponent>(entity, true);
        }
    }
}
