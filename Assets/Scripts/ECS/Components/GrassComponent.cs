using Unity.Entities;

public struct GrassComponent : IComponentData
{
    public bool watered;
    public int daisies;
    public float growSpeed;
    public float wateredMultiplier;
}