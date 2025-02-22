using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GrassBladeController : Interactible
{
    private float desiredSize = 1f;
    private float growSpeed = 0.01f;
    private float currentSize = 0.01f;
    private float wateredMultiplier = 10.0f;

    private int enteredWaters = 0;
    private bool watered = false;
    private bool FinishedGrowing { get { return currentSize >= desiredSize; } }
    private int daisies = 0;

    private new void Awake()
    {
        this.transform.localScale = new Vector3(currentSize, currentSize, currentSize);

        base.Awake();
    }

    private void Update()
    {
        if(currentSize < 1f)
        {
            if(watered)
            {
                currentSize += growSpeed * Time.deltaTime * wateredMultiplier;
            }
            else
            {
                currentSize += growSpeed * Time.deltaTime;
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
        if(!FinishedGrowing) return;
        outline.enabled = false;
    }

    public override void onInteract()
    {
        if(FinishedGrowing)
        {
            GameManager.Instance.player.GrassBlades++;
            while(UnityEngine.Random.Range(0, 101) < 5 * daisies)
            {
                GameManager.Instance.player.GrassBlades++;
            }
            GameManager.Instance.ITT.RemoveGrassBlade(this.transform.position.x, this.transform.position.y, this.transform.position.z);
            Destroy(gameObject);
        }
    }
}
