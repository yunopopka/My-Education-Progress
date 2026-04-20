using UnityEngine;
using System.Collections;

public class ReinforcementDrop : MonoBehaviour
{
    public Transform dropPoint;
    public Transform Target;
    public GameObject dronePrefab;

    private bool isDropping = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isDropping)
        {
            StartCoroutine(DropSequence());
        }
    }

    private IEnumerator DropSequence()
    {
        isDropping = true;
        Debug.Log("Запрос подтвержден, ожидайте");

        yield return new WaitForSeconds(2f);

        Debug.Log("Подкрепление прибыло!");

        for (int i = 0; i < 3; i++)
        {
            GameObject currentCube = Instantiate(dronePrefab, dropPoint.position, dropPoint.rotation);
            if (currentCube.TryGetComponent(out AiAgent comp)) { comp.TakeTarget(Target); }
            yield return new WaitForSeconds(0.5f);
        }
        
        Debug.Log("Высадка завершена");
        isDropping = false;
    }
}
