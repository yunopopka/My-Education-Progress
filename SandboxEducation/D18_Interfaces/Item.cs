using UnityEngine;

public class Item : MonoBehaviour , IInteractable
{
    public string itemName;

    public virtual void UseItem()
    {
        Debug.Log("Predmet Used");
    }

    public void Interact()
    {
        UseItem();
    }
}
