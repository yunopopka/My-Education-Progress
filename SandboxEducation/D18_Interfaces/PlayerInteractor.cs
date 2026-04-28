using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if(Physics.Raycast(ray,out hitInfo,5f))
            {
                if(hitInfo.collider.TryGetComponent(out IInteractable interactableObject))
                {
                    interactableObject.Interact();
                }
            }
        }
    }
}
