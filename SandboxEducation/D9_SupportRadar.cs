using UnityEngine;

public class SupportRadar : MonoBehaviour
{
    public Transform playerTank;
    public float scanRadius = 20f;
    public float repairDelay = 5f;
    public float speed = 5f;

    bool isRepairing = false;
    float repairTimer = 5f;
    Quaternion baseRotation;

    private void Start()
    {
        baseRotation = transform.rotation;
    }

    private void Update()
    {
        Scan();
        LookAndRepair();
    }

    private void Scan()
    {
        if(Vector3.Distance(transform.position,playerTank.position) >= scanRadius)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, baseRotation, speed * Time.deltaTime);
            isRepairing = false;
        }
    }

    private void LookAndRepair()
    {
        if(Vector3.Distance(transform.position, playerTank.position) <= scanRadius)
        {
            Vector3 currentVector = playerTank.position - transform.position;
            Quaternion Rotation = Quaternion.LookRotation(currentVector);

            transform.rotation = Quaternion.Slerp(transform.rotation, Rotation , speed * Time.deltaTime);

            isRepairing = true;
            repairTimer -= Time.deltaTime;

            if(repairTimer <= 0)
            {
                repairTimer = repairDelay;
                Debug.Log("Tank is repaired!");
            }
        }
    }
}
