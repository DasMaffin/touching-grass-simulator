using Unity.Entities;
using UnityEngine;

public class WeatherMonoBehaviour : MonoBehaviour
{
    public float rainIntensity;
    public class Baker : Baker<WeatherMonoBehaviour>
    {
        public override void Bake(WeatherMonoBehaviour authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic); // TODO try for a better flag.
            AddComponent(entity, new WeatherComponent
            {
                rainIntensity = authoring.rainIntensity
            });
        }
    }
}
