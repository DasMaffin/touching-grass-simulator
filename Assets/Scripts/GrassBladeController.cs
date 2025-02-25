using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GrassBladeController : Interactible
{
    #region resetValues

    // These values need to be reset after returning to the pool.
    public bool watered = false;
    public float currentSize = 0.01f;
    public int daisies = 0;

    #endregion

    public int selectedGrassSkin = 0;

    private float desiredSize = 1f;
    private float growSpeed = 0.01f;
    private float wateredMultiplier = 10.0f;

    private int enteredWaters = 0;
    private bool FinishedGrowing { get { return currentSize >= desiredSize; } }

    private new void Awake()
    {
        this.transform.localScale = new Vector3(currentSize, currentSize, currentSize);

        base.Awake();
    }

    private void Update()
    {
        if(currentSize < 1f)
        {
            float weatherMult = WeatherManager.Instance.GetGrowthMultiplier(watered);
            if(weatherMult == -1)
            {
                return;
            }
            print("Intended Growth multiplier: " + WeatherManager.Instance.GetGrowthMultiplier(enteredWaters != 0));
            print("Actual Growth speed: " + growSpeed);
            if(watered)
            {
                currentSize += growSpeed * Time.deltaTime * wateredMultiplier * weatherMult;
            }
            else
            {
                currentSize += growSpeed * Time.deltaTime * weatherMult;
            }
        }
        else
        {
            currentSize = 1f;
        }
        this.transform.localScale = new Vector3(currentSize, currentSize, currentSize);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Watered"))
        {
            enteredWaters++;
            watered = true;
        }
        switch(other.gameObject.GetComponent<FlowerController>()?.Flower)
        {
            case Flower.Daisy:
                daisies++;
                break;
            case Flower.Dandelion:
                break;
            case Flower.None:
                break;
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
    }

    public override void OnHoverExit()
    {
        outline.enabled = false;
    }

    public override void onInteract()
    {
        if(FinishedGrowing)
        {
            InventoryManager.Instance.AddItem(Item.GrassBlades, 1);
            while(UnityEngine.Random.Range(0, 101) < 5 * daisies)
            {
                InventoryManager.Instance.AddItem(Item.GrassBlades, 1);
            }
            GameManager.Instance.ITT.RemoveGrassBlade(this.transform.position.x, this.transform.position.y, this.transform.position.z);

            GameManager.Instance.grassPools[selectedGrassSkin].Release(this.gameObject);
        }
    }
}
