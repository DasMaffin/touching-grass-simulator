using System;
using UnityEngine;

public class GrassPreviewController : MonoBehaviour
{
    public static GrassPreviewController ActiveController = null;
    public bool canBePlaced { get => activeTriggers == 0; }

    [SerializeField] private Material canPlaceMaterial;
    [SerializeField] private Material cantPlaceMaterial;
    [SerializeField] private int activeTriggers = 0;

    private MeshRenderer[] myMeshRenderers;

    private void Start()
    {
        myMeshRenderers = GetComponentsInChildren<MeshRenderer>();
        UpdateMeshes(canPlaceMaterial);
        this.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        ActiveController = this;
    }

    private void OnDisable()
    {
        activeTriggers = 0;
        ActiveController = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer != LayerMask.NameToLayer("NPC Interactible") && other.gameObject.layer != LayerMask.NameToLayer("NPC Noninteractible")) return;
        activeTriggers++;
        if(!canBePlaced)
            UpdateMeshes(cantPlaceMaterial);
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer != LayerMask.NameToLayer("NPC Interactible") && other.gameObject.layer != LayerMask.NameToLayer("NPC Noninteractible")) return;
        activeTriggers--;
        if(canBePlaced)
            UpdateMeshes(canPlaceMaterial);
    }

    private void UpdateMeshes(Material mat)
    {
        foreach(MeshRenderer renderer in myMeshRenderers)
        {
            renderer.material = mat;
        }
    }

    private RaycastHit hit;
    private void Update()
    {
        LayerMask layersToCheck = (1 << 0) | (1 << 1); // 1 << layer
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        if(Physics.Raycast(ray, out hit, 4, layersToCheck))
        {
            this.transform.position = new Vector3(hit.point.x, hit.point.y - 0.001f, hit.point.z);
        }
        else
        {
            this.transform.position = new Vector3(0, -1000, 0);
        }
    }

    internal void PlantGrass(InventoryItem ii)
    {
        if(canBePlaced && hit.collider != null)
        {
            GameManager.Instance.ITT.HandleGrassBlade(hit, ii);
        }
    }
}
