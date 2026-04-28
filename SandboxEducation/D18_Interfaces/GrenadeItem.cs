using UnityEngine;

public class GrenadeItem : Item
{
    public override void UseItem()
    {
        Debug.Log($"Брошена граната: {itemName}!"); 
        Destroy(gameObject);
    }
}
