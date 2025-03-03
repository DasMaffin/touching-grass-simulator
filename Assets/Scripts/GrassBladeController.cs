using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class GrassBladeController : Interactible
{
    #region resetValues

    // These values need to be reset after returning to the pool.
    public bool watered = false;
    public float currentSize = 0.01f;
    public List<FlowerController> affectingFlowers = new List<FlowerController>();

    #endregion

    public float growSpeed = 0.01f;
    public int selectedGrassSkin = 0;
    public float wateredMultiplier = 10.0f;
    public Transform growObject;

    private float desiredSize = 1f;
    private int enteredWaters = 0;

    private bool FinishedGrowing { get { return currentSize >= desiredSize; } }

    private new void Awake()
    {
        this.growObject.localScale = new Vector3(currentSize, currentSize, currentSize);

        base.Awake();
    }

    private void OnEnable()
    {
        GrassBladeManager.Instance?.RegisterBlade(this);
        UpdateScale();
    }

    private void OnDisable()
    {
        GrassBladeManager.Instance?.UnregisterBlade(this);
    }

    public void UpdateScale()
    {
        growObject.localScale = new Vector3(currentSize, currentSize, currentSize);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Watered"))
        {
            enteredWaters++;
            watered = true;
        }
        else if(other.gameObject.layer == LayerMask.NameToLayer("NPC Noninteractible") && GameManager.Instance.flowerCache[other.gameObject].FinishedGrowing)
        {
            affectingFlowers.Add(GameManager.Instance.flowerCache[other.gameObject]);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Watered"))
        {
            enteredWaters--;
            if(enteredWaters == 0)
                watered = false;
        }
    }

    public override void OnHoverEnter()
    {
        if(!FinishedGrowing) return;
        outline.enabled = true;
        base.OnHoverEnter();
    }

    public override void OnHoverExit()
    {
        outline.enabled = false;
        base.OnHoverExit();
    }

    public override void onInteract()
    {
        if(FinishedGrowing)
        {
            InventoryManager.Instance.AddItem(Item.GrassBlades, 1);
            float chance = affectingFlowers.Sum(f => 
            {
                if(f.currentSize >= f.desiredSize)
                    return f.bonusDropChanceAdditive;
                else 
                    return 0;
            });
            while(UnityEngine.Random.Range(0, 101) < chance)
            {
                InventoryManager.Instance.AddItem(Item.GrassBlades, 1);
                chance /= 5;
            }
            GameManager.Instance.ITT.RemoveGrassBlade(this.transform.position.x, this.transform.position.y, this.transform.position.z);

            GameManager.Instance.grassPools[selectedGrassSkin].Release(this.gameObject);
        }        
    }
}