using UnityEngine;

public class PatrolBomber : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;

    public float speed = 5f;
    public GameObject bombPrefab;
    public float dropInterval = 2f;

    public float dropTimer = 0f;
    private bool movingToB = true;

    private void Update()
    {
        Bombing();

        if (movingToB == true)
        {
            ComingB();
        }
        else
        {
            ComingA();
        }
    }

    private void Bombing()
    {
        dropTimer -= Time.deltaTime;

        if(dropTimer <= 0)
        {
            Instantiate(bombPrefab,transform.position,transform.rotation);
            dropTimer = dropInterval;
        }
    }

    private void ComingB()
    {

            transform.position = Vector3.MoveTowards(transform.position, pointB.position, speed * Time.deltaTime);

            if(Vector3.Distance(transform.position,pointB.position) <= 0.1f)
            {
                movingToB = false;
           
            }
    }

    private void ComingA()
    {
        transform.position = Vector3.MoveTowards(transform.position, pointA.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, pointA.position) <= 0.1f)
        {
            movingToB = true;

        }
    }
}
