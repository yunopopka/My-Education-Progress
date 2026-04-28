using UnityEngine;

public class Radio : MonoBehaviour , IInteractable
{
    public void Interact()
    {
        Debug.Log("Играет музыка: Группа Кино - Пачка сигарет");
    }
}
