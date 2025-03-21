using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using System.Collections.Generic;

public class GrassBladeManager : MonoBehaviour
{
    public static GrassBladeManager Instance { get; private set; }

    private NativeArray<float> currentSizes;
    private NativeArray<bool> wateredStates;
    private NativeArray<float> growSpeeds;
    private NativeArray<float> wateredMultipliers;
    private NativeArray<float> flowerGrowthMultipliers;
    private bool isInitialized;

    public List<GrassBladeController> activeGrassBlades = new List<GrassBladeController>(); // TODO Change this to use GameManager.Instance.activeGrassBlade (It may update too early right now)

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        DisposeNativeArrays();
    }

    public void RegisterBlade(GrassBladeController blade)
    {
        activeGrassBlades.Add(blade);
        UpdateNativeArraysCapacity();
    }

    public void UnregisterBlade(GrassBladeController blade)
    {
        activeGrassBlades.Remove(blade);
        UpdateNativeArraysCapacity();
    }

    private void UpdateNativeArraysCapacity()
    {
        DisposeNativeArrays();
        InitializeNativeArrays(activeGrassBlades.Count);
    }

    private void InitializeNativeArrays(int capacity)
    {
        if(capacity <= 0) return;

        currentSizes = new NativeArray<float>(capacity, Allocator.Persistent);
        wateredStates = new NativeArray<bool>(capacity, Allocator.Persistent);
        growSpeeds = new NativeArray<float>(capacity, Allocator.Persistent);
        wateredMultipliers = new NativeArray<float>(capacity, Allocator.Persistent);
        flowerGrowthMultipliers = new NativeArray<float>(capacity, Allocator.Persistent);
        isInitialized = true;
    }

    private void DisposeNativeArrays()
    {
        if(!isInitialized) return;

        currentSizes.Dispose();
        wateredStates.Dispose();
        growSpeeds.Dispose();
        wateredMultipliers.Dispose(); 
        flowerGrowthMultipliers.Dispose();
        isInitialized = false;
    }

    private JobHandle handle;
    private void Update()
    {
        if(!isInitialized || activeGrassBlades.Count == 0) return;

        float wateredWeatherMult = WeatherManager.Instance.GetGrowthMultiplier(true);
        float nonWateredWeatherMult = WeatherManager.Instance.GetGrowthMultiplier(false);

        // Copy data to NativeArrays
        for(int i = 0; i < activeGrassBlades.Count; i++)
        {
            GrassBladeController blade = activeGrassBlades[i];
            currentSizes[i] = blade.currentSize;
            wateredStates[i] = blade.watered;
            growSpeeds[i] = blade.growSpeed;
            wateredMultipliers[i] = blade.wateredMultiplier;
            flowerGrowthMultipliers[i] = CalculateFlowerBonus(blade.affectingFlowers);
        }

        // Schedule and complete job
        var job = new GrowthJob
        {
            CurrentSizes = currentSizes,
            WateredStates = wateredStates,
            GrowSpeeds = growSpeeds,
            WateredMultipliers = wateredMultipliers,
            FlowerGrowthMultipliers = flowerGrowthMultipliers,
            DeltaTime = Time.deltaTime,
            WateredWeatherMultiplier = wateredWeatherMult,
            NonWateredWeatherMultiplier = nonWateredWeatherMult
        };

        handle = job.Schedule(activeGrassBlades.Count, 64);
        handle.Complete();

        // Update blades with new values
        for(int i = 0; i < activeGrassBlades.Count; i++)
        {
            GrassBladeController blade = activeGrassBlades[i];
            blade.currentSize = currentSizes[i];
            blade.UpdateScale();
        }
    }

    private float CalculateFlowerBonus(List<FlowerController> flowers)
    {
        if(flowers == null || flowers.Count == 0) return 1f;

        float posSum = 1f;
        int negCount = 0;

        foreach(var flower in flowers)
        {
            if(flower.bonusGrowthSpeedAdditive >= 1f)
            {
                posSum += flower.bonusGrowthSpeedAdditive - 1f;
            }
            else if(flower.bonusGrowthSpeedAdditive > 0f)
            {
                negCount++;
            }
        }

        float negSum = Mathf.Pow(0.9f, negCount); // Apply the new formula

        float sum = posSum * negSum;

        return sum;
    }
}

[BurstCompile]
public struct GrowthJob : IJobParallelFor
{
    public NativeArray<float> CurrentSizes;
    [ReadOnly] public NativeArray<bool> WateredStates;
    [ReadOnly] public NativeArray<float> GrowSpeeds;
    [ReadOnly] public NativeArray<float> WateredMultipliers; 
    [ReadOnly] public NativeArray<float> FlowerGrowthMultipliers;
    [ReadOnly] public float DeltaTime;
    [ReadOnly] public float WateredWeatherMultiplier;
    [ReadOnly] public float NonWateredWeatherMultiplier;

    public void Execute(int index)
    {
        float currentSize = CurrentSizes[index];
        if(currentSize >= 1f) return;

        bool watered = WateredStates[index];
        float weatherMult = watered ? WateredWeatherMultiplier : NonWateredWeatherMultiplier;

        if(weatherMult == -1f) return;

        float growth = GrowSpeeds[index] * DeltaTime * weatherMult;
        if(watered)
        {
            growth *= WateredMultipliers[index];
        }

        growth *= FlowerGrowthMultipliers[index];

        currentSize = Mathf.Min(currentSize + growth, 1f);
        CurrentSizes[index] = currentSize;
    }
}