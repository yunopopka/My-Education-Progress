using UnityEngine;

public class Door : MonoBehaviour , IInteractable
{
    public void Interact()
    {
        Debug.Log("Door is open");
    }
}
