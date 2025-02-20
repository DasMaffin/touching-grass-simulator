using System.Collections.Generic;
using UnityEngine;

public class InteractiveTerrainTexture : MonoBehaviour
{
    public Terrain terrain; // Assign your terrain in the Inspector
    public GameObject grassBlade; // Assign your grass blade prefab in the Inspector
    public float clickRadius = 1.0f; // Radius of the click's effect

    private TerrainData terrainData;
    private int alphamapWidth;
    private int alphamapHeight;

    void Start()
    {
        // Get terrain data
        if(terrain == null)
        {
            Debug.LogError("No terrain assigned to InteractiveTerrainTexture!");
            return;
        }

        terrainData = terrain.terrainData;
        alphamapWidth = terrainData.alphamapWidth;
        alphamapHeight = terrainData.alphamapHeight;

        ResetAlphaMap();
    }

    void Update()
    {
        // Check for left mouse button click
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hit, 4))
            {
                if(hit.collider.gameObject == terrain.gameObject)
                {
                    HandleGrassBlade(hit.point);
                }
            }
        }
    }

    private void ResetAlphaMap()
    {
        // Initialize a new alpha map array
        float[,,] alphaMaps = new float[alphamapWidth, alphamapHeight, terrainData.alphamapLayers];

        // Set the default layer (layer 0) to fully cover the terrain
        for(int z = 0; z < alphamapHeight; z++)
        {
            for(int x = 0; x < alphamapWidth; x++)
            {
                alphaMaps[z, x, 0] = 1.0f; // Fully assign default layer
                alphaMaps[z, x, 1] = 0.0f; // Transition layer starts at 0
            }
        }

        // Apply the reset alpha map to the terrain
        terrainData.SetAlphamaps(0, 0, alphaMaps);
    }

    private void HandleGrassBlade(Vector3 hitPoint)
    {
        // Translate world coordinates to terrain alpha map coordinates
        //Vector3 terrainPosition = hitPoint - terrain.transform.position;

        if(GameManager.Instance.GrassSeeds > 0)
        {
            SpawnGrassBlade(hitPoint.x, hitPoint.z);
            GameManager.Instance.GrassSeeds--;
        }
    }

    private void SpawnGrassBlade(float x, float z)
    {
        Instantiate(grassBlade, new Vector3(x, 0, z), Quaternion.identity);

        UpdateAlphaMap((int)x, (int)z, 0.1f);
    }

    public void RemoveGrassBlade(float x, float z)
    {
        UpdateAlphaMap((int)x, (int)z, -0.1f);
    }

    private void UpdateAlphaMap(int x, int z, float changeAmount)
    {
        Vector3 terrainPosition = new Vector3(x, 0, z) - terrain.transform.position;
        x = Mathf.RoundToInt((terrainPosition.x / terrainData.size.x) * alphamapWidth);
        z = Mathf.RoundToInt((terrainPosition.z / terrainData.size.z) * alphamapHeight);
        // Get the alpha map
        float[,,] alphaMaps = terrainData.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);

        // Apply the change only to the clicked position
        if(x >= 0 && x < alphamapWidth && z >= 0 && z < alphamapHeight)
        {
            // Update the transition layer (layer 1) based on grass presence
            alphaMaps[z, x, 1] = Mathf.Clamp01(alphaMaps[z, x, 1] + changeAmount);

            // Normalize layers to ensure the sum is 1
            float total = alphaMaps[z, x, 0] + alphaMaps[z, x, 1];
            alphaMaps[z, x, 0] = Mathf.Clamp01(alphaMaps[z, x, 0] + (1.0f - total));
            alphaMaps[z, x, 1] = Mathf.Clamp01(alphaMaps[z, x, 1]);

            // Apply the modified alpha map back to the terrain
            terrainData.SetAlphamaps(0, 0, alphaMaps);
        }
    }
}
