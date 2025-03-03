using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class FlowerManager : MonoBehaviour
{
    public static FlowerManager Instance { get; private set; }

    private NativeArray<float> currentSizes;
    private NativeArray<bool> wateredStates;
    private NativeArray<float> growSpeeds;
    private NativeArray<float> wateredMultipliers;
    private bool isInitialized;

    public List<FlowerController> activeFlowers = new List<FlowerController>(); // TODO Change this to use GameManager.Instance.activeGrassBlade (It may update too early right now)
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

    public void RegisterBlade(FlowerController fc)
    {
        activeFlowers.Add(fc);
        UpdateNativeArraysCapacity();
    }

    public void UnregisterBlade(FlowerController fc)
    {
        activeFlowers.Remove(fc);
        UpdateNativeArraysCapacity();
    }

    private void UpdateNativeArraysCapacity()
    {
        DisposeNativeArrays();
        InitializeNativeArrays(activeFlowers.Count);
    }

    private void InitializeNativeArrays(int capacity)
    {
        if(capacity <= 0) return;

        currentSizes = new NativeArray<float>(capacity, Allocator.Persistent);
        wateredStates = new NativeArray<bool>(capacity, Allocator.Persistent);
        growSpeeds = new NativeArray<float>(capacity, Allocator.Persistent);
        wateredMultipliers = new NativeArray<float>(capacity, Allocator.Persistent);
        isInitialized = true;
    }

    private void DisposeNativeArrays()
    {
        if(!isInitialized) return;

        currentSizes.Dispose();
        wateredStates.Dispose();
        growSpeeds.Dispose();
        wateredMultipliers.Dispose();
        isInitialized = false;
    }

    private JobHandle handle;
    private void Update()
    {
        if(!isInitialized || activeFlowers.Count == 0) return;

        // Copy data to NativeArrays
        for(int i = 0; i < activeFlowers.Count; i++)
        {
            FlowerController fc = activeFlowers[i];
            currentSizes[i] = fc.currentSize;
            wateredStates[i] = fc.watered;
            growSpeeds[i] = fc.growSpeed;
            wateredMultipliers[i] = fc.wateredMultiplier;
        }

        // Get weather multipliers
        float wateredWeatherMult = WeatherManager.Instance.GetGrowthMultiplier(true);
        float nonWateredWeatherMult = WeatherManager.Instance.GetGrowthMultiplier(false);

        // Schedule and complete job
        var job = new FlowerGrowthJob
        {
            CurrentSizes = currentSizes,
            WateredStates = wateredStates,
            GrowSpeeds = growSpeeds,
            WateredMultipliers = wateredMultipliers,
            DeltaTime = Time.deltaTime,
            WateredWeatherMultiplier = wateredWeatherMult,
            NonWateredWeatherMultiplier = nonWateredWeatherMult
        };

        handle = job.Schedule(activeFlowers.Count, 64);
        handle.Complete();

        // Update blades with new values
        for(int i = 0; i < activeFlowers.Count; i++)
        {
            FlowerController fc = activeFlowers[i];
            fc.currentSize = currentSizes[i];
            fc.UpdateScale();
        }
    }
}

[BurstCompile]
public struct FlowerGrowthJob : IJobParallelFor
{
    public NativeArray<float> CurrentSizes;
    [ReadOnly] public NativeArray<bool> WateredStates;
    [ReadOnly] public NativeArray<float> GrowSpeeds;
    [ReadOnly] public NativeArray<float> WateredMultipliers;

    public float DeltaTime;
    public float WateredWeatherMultiplier;
    public float NonWateredWeatherMultiplier;

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

        currentSize = Mathf.Min(currentSize + growth, 1f);
        CurrentSizes[index] = currentSize;
    }
}

