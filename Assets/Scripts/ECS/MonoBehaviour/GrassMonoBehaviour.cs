using Unity.Entities;
using UnityEngine;

public class GrassMonoBehaviour : MonoBehaviour
{
    public bool watered = false;
    public int daisies = 0;
    public float growSpeed = 0.01f;
    public float wateredMultiplier = 10.0f;
    public float rainIntensity = 0.0f;

    public class Baker : Baker<GrassMonoBehaviour>
    {
        public override void Bake(GrassMonoBehaviour authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic); // TODO try for a better flag.
            AddComponent(entity, new GrassComponent
            {
                watered = authoring.watered,
                daisies = authoring.daisies,
                growSpeed = authoring.growSpeed,
                wateredMultiplier = authoring.wateredMultiplier
            });
        }
    }
}
