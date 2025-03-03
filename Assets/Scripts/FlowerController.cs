using NUnit.Framework.Constraints;
using System.Collections.Generic;
using UnityEngine;

public class FlowerController : MonoBehaviour
{
    public FlowerType Flower;
    public GameObject defaultObject;
    public GameObject inRangeObject;
    public Outline outline;

    [SerializeField] private List<Collider> collidersInTrigger = new List<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Preview"))
        {
            collidersInTrigger.Add(other);
            defaultObject.SetActive(false);
            inRangeObject.SetActive(true);
            outline.enabled = true;
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
    }
}
