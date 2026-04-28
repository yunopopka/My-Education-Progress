using UnityEngine;

public class FlashlightItem : Item
{
    public override void UseItem()
    {
        Debug.Log($"Фонарик {itemName} включен!");
    }
}
