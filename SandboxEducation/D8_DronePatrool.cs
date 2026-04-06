using UnityEngine;


public class LearningScript : MonoBehaviour
{
    public Transform target;
    public float aggroRadius = 10f;
    public float speed = 5f;
    private Vector3 homePosition;
    private bool isChasing = false;

    private void Start() { homePosition = transform.position; }

    private void Update()
    {
        Scan();
        if (isChasing == true)
        {
            Attack();
        }
        else { Back(); }
    }

    private void Scan()
    {
        if( Vector3.Distance( transform.position,target.position ) <= aggroRadius )
        {
            isChasing = true;
        }
        else { isChasing = false; }
    }

    private void Attack()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        Debug.Log("Drone is coming!");
    }

    private void Back() { transform.position = Vector3.MoveTowards(transform.position, homePosition, speed * Time.deltaTime); }
}
