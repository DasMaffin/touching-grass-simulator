using Unity.Entities;
using UnityEngine;

class IsActiveMonoBehaviour : MonoBehaviour
{
    public GameObject thisObject;

    public class Baker : Baker<IsActiveMonoBehaviour>
    {
        public override void Bake(IsActiveMonoBehaviour authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new IsActiveComponent
            {
                thisEntity = GetEntity(authoring.thisObject, TransformUsageFlags.Dynamic)
            });
            SetComponentEnabled<IsActiveComponent>(entity, false);
        }
    }
}
