using UnityEngine;

public class ProximityMine : MonoBehaviour
{
    public Transform target;
    public float triggerRadius = 5f;
    public float fuseTime = 2f;
    public GameObject exposionPrefab;
    private bool isTriggered = false;

    private void Update()
    {
        if (isTriggered == false)
        {
            Scan();
        }
        else
        {
            Attack(); 
        }
    }

    private void Scan()
    {

        if (Vector3.Distance(transform.position, target.position) <= triggerRadius)
        {
            isTriggered = true; 
            Debug.Log("P p p...");

        }
    }

    private void Attack()
    {

        fuseTime -= Time.deltaTime;

        if (fuseTime <= 0)
        {

            Instantiate(exposionPrefab, transform.position, transform.rotation);
            Debug.Log("Boom!");
            Destroy(gameObject);
        }
    }

}
