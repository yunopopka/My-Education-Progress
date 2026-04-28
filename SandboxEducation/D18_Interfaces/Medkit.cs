using UnityEngine;

public class Medkit : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Medkit taked!");
        Destroy(gameObject);
    }
}
