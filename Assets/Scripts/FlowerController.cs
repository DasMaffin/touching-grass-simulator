using NUnit.Framework.Constraints;
using System.Collections.Generic;
using UnityEngine;

public class FlowerController : Interactible
{
    public FlowerType Flower;
    public GameObject defaultObject;
    public GameObject inRangeObject;
    public Transform growObject;

    [SerializeField] private List<Collider> collidersInTrigger = new List<Collider>();

    #region effects

    public int bonusDropChanceAdditive = 0; // The chance as a % value that there will be a bonus drop. Additive - value of 2 with 10 flowers: 20.
    public float bonusGrowthSpeedAdditive = 0; // The multiplier to growth speed. Use fractions to make slower (e.g. 1/4 for .25x speed). Additive - value of 1/4 with 10 flowers: 1/40. Formula: 1/SUM(1/value))

    #endregion

    internal int poolIndex;
    internal bool watered = false;
    internal float currentSize = 0.01f;
    internal float desiredSize = 1f;
    internal float growSpeed = 0.01f;
    internal float wateredMultiplier = 10f;

    private int enteredWaters = 0;

    public bool FinishedGrowing { get { return currentSize >= desiredSize; } }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Preview"))
        {
            collidersInTrigger.Add(other);
            if(FinishedGrowing)
            {
                defaultObject.SetActive(false);
                inRangeObject.SetActive(true);
                outline.enabled = true;
            }
        }
        if(other.gameObject.layer == LayerMask.NameToLayer("Watered"))
        {
            enteredWaters++;
            watered = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Preview"))
        {
            collidersInTrigger.Remove(other);
            if(collidersInTrigger.Count <= 0)
            {
                defaultObject.SetActive(true);
                inRangeObject.SetActive(false);
                outline.enabled = false;
            }
        }
        if(other.gameObject.layer == LayerMask.NameToLayer("Watered"))
        {
            enteredWaters--;
            if(enteredWaters == 0)
                watered = false;
        }
    }

    private void OnEnable()
    {
        FlowerManager.Instance?.RegisterBlade(this);
        UpdateScale();
    }

    private void OnDisable()
    {
        FlowerManager.Instance?.UnregisterBlade(this);
    }

    public void UpdateScale()
    {
        growObject.localScale = new Vector3(currentSize, currentSize, currentSize);
    }

    public override void onInteract()
    {
    }

    public override void OnHoverEnter()
    {
        base.OnHoverEnter();
    }

    public override void OnHoverExit()
    {
        base.OnHoverExit();
    }
}
