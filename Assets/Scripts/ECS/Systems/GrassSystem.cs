using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

partial struct GrassSystem : ISystem
{
    //[BurstCompile]
    //public void OnUpdate(ref SystemState state)
    //{
    //    if(!SystemAPI.TryGetSingleton<WeatherComponent>(out WeatherComponent weatherData))
    //    {
    //        return;
    //    }
    //    GrassGrowthJob grassGrowthJob = new GrassGrowthJob
    //    {
    //        deltaTime = SystemAPI.Time.DeltaTime,
    //        rainIntensity = weatherData.rainIntensity
    //    };
    //    grassGrowthJob.ScheduleParallel();
    //}
}

//[BurstCompile]
//public partial struct GrassGrowthJob : IJobEntity
//{
//    public float deltaTime;
//    public float rainIntensity;

//    public void Execute(ref LocalTransform localTransform, GrassComponent grassComponent)
//    {
//        if(localTransform.Scale < 1f)
//        {
//            float weatherMult = GetGrowthMultiplier(grassComponent.watered, rainIntensity);
//            if(weatherMult == -1)
//            {
//                return;
//            }
//            if(grassComponent.watered)
//            {
//                localTransform.Scale += grassComponent.growSpeed * deltaTime * grassComponent.wateredMultiplier * weatherMult;
//            }
//            else
//            {
//                localTransform.Scale += grassComponent.growSpeed * deltaTime * weatherMult;
//            }
//        }
//        else
//        {
//            localTransform.Scale = 1f;
//        }
//    }

//    private float GetGrowthMultiplier(bool isWatered, float rainIntensity)
//    {
//        if(isWatered)
//        {
//            if(rainIntensity > 0.67f) return -1f; // Drown
//            if(rainIntensity > 0.33f) return 1f - ((rainIntensity - 0.33f) / 0.3399f); // 1x to 0x growth
//            return 1f; // Constant 1x growth
//        }
//        else
//        {
//            if(rainIntensity > 0.9f) return -1f; // Drown
//            if(rainIntensity > 0.75f) return (0.9f - rainIntensity) / 0.15f; // 1x to 0x growth
//            if(rainIntensity > 0.5f) return 1f + (10f - 1f) * ((0.75f - rainIntensity) / 0.25f); // 10x to 1x growth
//            return 1f + (10f - 1f) * (rainIntensity / 0.5f); // 1x to 10x growth
//        }
//    }
//}
