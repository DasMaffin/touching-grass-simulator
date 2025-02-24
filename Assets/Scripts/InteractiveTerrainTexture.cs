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

    private void ResetAlphaMap()
    {
        float[,,] alphaMaps = new float[alphamapWidth, alphamapHeight, terrainData.alphamapLayers];

        for(int z = 0; z < alphamapHeight; z++)
        {
            for(int x = 0; x < alphamapWidth; x++)
            {
                // Convert alpha map indices to world position
                float normX = (float)x / (alphamapWidth - 1);
                float normZ = (float)z / (alphamapHeight - 1);
                Vector3 worldPos = new Vector3(
                    normX * terrainData.size.x + terrain.transform.position.x,
                    0,
                    normZ * terrainData.size.z + terrain.transform.position.z
                );

                bool isWatered = IsPointInsideCollider(worldPos);

                // Reset all layers to 0
                for(int layer = 0; layer < terrainData.alphamapLayers; layer++)
                {
                    alphaMaps[z, x, layer] = 0f;
                }

                // Set the appropriate layer based on collision
                if(isWatered)
                {
                    alphaMaps[z, x, 3] = 1f; // Grass_Soil_A layer (index 3)
                }
                else
                {
                    alphaMaps[z, x, 0] = 1f; // Dirt layer (index 0)
                }
            }
        }

        terrainData.SetAlphamaps(0, 0, alphaMaps);
    }

    bool IsPointInsideCollider(Vector3 point)
    {
        foreach(Collider collider in Physics.OverlapSphere(point, 0.1f, ~0, QueryTriggerInteraction.Collide))
        {
            //print(collider.gameObject.layer);
            if(collider.gameObject.layer == LayerMask.NameToLayer("Watered"))
            {
                return true;
            }
        }
        return false;
    }

    public void HandleGrassBlade(RaycastHit hit, InventoryItem ii)
    {
        if(hit.collider.gameObject == terrain.gameObject)
        {
            HandleGrassBlade(hit.point, ii);
        }
    }

    private void HandleGrassBlade(Vector3 hitPoint, InventoryItem ii)
    {
        if(InventoryManager.Instance.GetItemCount(Item.GrassSeeds) > 0)
        {
            SpawnGrassBlade(hitPoint.x, hitPoint.y, hitPoint.z);
            InventoryManager.Instance.RemoveItem(Item.GrassSeeds, 1, ii);
        }
    }

    private void SpawnGrassBlade(float x, float y, float z)
    {
        GameManager.Instance.InstantiateGrass(new Vector3(x, y, z));

        UpdateAlphaMap(x, y, z, 0.1f);
    }

    public void RemoveGrassBlade(float x, float y, float z)
    {
        UpdateAlphaMap(x, y, z, -0.1f);
    }

    private void UpdateAlphaMap(float x, float y, float z, float changeAmount)
    {
        Vector3 hitPoint = new Vector3(x, y, z);

        Vector3 terrainPosition = new Vector3(x, 0, z) - terrain.transform.position;
        x = Mathf.RoundToInt((terrainPosition.x / terrainData.size.x) * alphamapWidth);
        z = Mathf.RoundToInt((terrainPosition.z / terrainData.size.z) * alphamapHeight);
        // Get the alpha map
        float[,,] alphaMaps = terrainData.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);

        // Apply the change only to the clicked position
        if(x >= 0 && x < alphamapWidth && z >= 0 && z < alphamapHeight)
        {
            int mapToUseIndex = 0;
            foreach(Collider collider in Physics.OverlapSphere(hitPoint, 0.1f, ~0, QueryTriggerInteraction.Collide))
            {
                if(collider.gameObject.layer == LayerMask.NameToLayer("Watered") && collider.gameObject.name != "WateringCanWateringArea")
                {
                    mapToUseIndex = 3;
                    break;
                }
            }
            // Update the transition layer (layer 1) based on grass presence
            alphaMaps[(int)z, (int)x, 1] = Mathf.Clamp01(alphaMaps[(int)z, (int)x, 1] + changeAmount);

            // Normalize layers to ensure the sum is 1
            float total = alphaMaps[(int)z, (int)x, mapToUseIndex] + alphaMaps[(int)z, (int)x, 1];
            alphaMaps[(int)z, (int)x, mapToUseIndex] = Mathf.Clamp01(alphaMaps[(int)z, (int)x, mapToUseIndex] + (1.0f - total));
            alphaMaps[(int)z, (int)x, 1] = Mathf.Clamp01(alphaMaps[(int)z, (int)x, 1]);

            // Apply the modified alpha map back to the terrain
            terrainData.SetAlphamaps(0, 0, alphaMaps);
        }
    }
}
