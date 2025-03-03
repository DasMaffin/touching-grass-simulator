using System;
using System.Collections.Generic;
using UnityEngine;

public class GrassPreviewController : MonoBehaviour
{
    public static GrassPreviewController ActiveController = null;
    public bool canBePlaced { get => disableTriggers.Count == 0; }

    [SerializeField] private Material canPlaceMaterial;
    [SerializeField] private Material cantPlaceMaterial;

    private MeshRenderer[] myMeshRenderers;

    [SerializeField] private List<Collider> collidersInTrigger = new List<Collider>();
    [SerializeField] private List<Collider> disableTriggers = new List<Collider>(); // Mind you if the other has 2 colliders it can be inside double.

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
        ActiveController = null;

        Collider myCol = this.GetComponent<Collider>();
        foreach(var collider in collidersInTrigger)
        {
            collider.GetComponentInParent<FlowerController>()?.OnTriggerExit(myCol);
        }
        disableTriggers.Clear();
        collidersInTrigger.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        collidersInTrigger.Add(other);
        if(other.gameObject.layer == LayerMask.NameToLayer("NPC Interactible"))
        {
            disableTriggers.Add(other);
        }
        if(!canBePlaced)
            UpdateMeshes(cantPlaceMaterial);
    }

    private void OnTriggerExit(Collider other)
    {
        collidersInTrigger.Remove(other);
        if(other.gameObject.layer == LayerMask.NameToLayer("NPC Interactible"))
        {
            disableTriggers.Remove(other);
        }
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
