using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GrassBladeController : Interactible
{
    private float desiredSize = 1f;
    private float growSpeed = 0.1f;
    private float currentSize = 0.01f;

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
            currentSize += growSpeed * Time.deltaTime;
        }
        else
        {
            currentSize = 1f;
        }
        this.transform.localScale = new Vector3(currentSize, currentSize, currentSize);
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
            GameManager.Instance.GrassBlades++;
            GameManager.Instance.ITT.RemoveGrassBlade(this.transform.position.x, this.transform.position.z);
            Destroy(gameObject);
        }
    }
}
