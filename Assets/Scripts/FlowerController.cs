using NUnit.Framework.Constraints;
using UnityEngine;

public enum Flower
{
    None = 0,
    Daisy = 1,
    Dandelion = 2
}

public class FlowerController : MonoBehaviour
{
    public Flower Flower;
}
