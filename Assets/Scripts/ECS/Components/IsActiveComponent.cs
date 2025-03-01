using Unity.Entities;

public struct IsActiveComponent : IComponentData, IEnableableComponent
{
    public Entity thisEntity;
}
