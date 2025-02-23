using System.Text.RegularExpressions;
using UnityEngine;

public class InventorySlotController : MonoBehaviour
{
    public bool isHotBar;
    public bool isFull = false;

    public int slotId;

    private void Awake()
    {
        if(isHotBar)
        {
            slotId = ExtractIdFromName(this.gameObject) + 40;
        }
        else
        {
            slotId = ExtractIdFromName(this.gameObject);
        }
    }

    public int ExtractIdFromName(GameObject obj)
    {
        if(obj == null || string.IsNullOrEmpty(obj.name))
            return 0;

        Match match = Regex.Match(obj.name, "\\((\\d+)\\)");

        return match.Success ? int.Parse(match.Groups[1].Value) : 0;
    }
}
