using UnityEngine;
using UnityEngine.UI;

public class WaterTankController : Building
{
    public float fillState = 0f;

    [SerializeField] private Slider fillSlider;
    private float maxCapacity = 100f;

    private new void Awake()
    {
        fillSlider.maxValue = maxCapacity;

        base.Awake();
    }

    private void Update()
    {
        if(fillState < maxCapacity)
        {
            fillState += WeatherManager.Instance.rs.RainIntensity * Time.deltaTime * 1/3; // 1/3 = 300s fill time or 1 fill every 3 seconds.
            fillSlider.value = fillState;
        }
        else if(fillState > maxCapacity)
        {
            fillState = maxCapacity;
        }
    }

    public override void OnHoverEnter()
    {
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
        if(InventoryManager.Instance.GetItemInSlot(InventoryBarManager.Instance.ActiveSlot + 40).Item == Item.WateringCan)
        {
            float reqWater = GameManager.Instance.player.MaxAvailableWater - GameManager.Instance.player.AvailableWater;
            if(reqWater > fillState)
            {
                GameManager.Instance.player.AvailableWater += fillState;
                fillState = 0;
            }
            else
            {
                fillState -= reqWater;
                GameManager.Instance.player.AvailableWater = GameManager.Instance.player.MaxAvailableWater;
            }
        }
    }
}
