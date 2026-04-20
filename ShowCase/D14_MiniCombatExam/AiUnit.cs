using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class AiUnit : MonoBehaviour
{
    private Transform target;
    private float Timer = 5f;

    public void TakeEnemy(Transform currentEnemy)
    {
        target = currentEnemy;
    }

    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (target != null && agent != null)
        {
            agent.SetDestination(target.position);
        }
    }

    private void Update()
    {
        Timer -= Time.deltaTime;

        if(Timer <= 0)
        {
            DistToTarg();
            Timer = 5f;
        }
    }

    private void DistToTarg()
    {
        if (Vector3.Distance(transform.position, target.position) <= 3f && gameObject != null)
        {
            gameObject.SetActive(false);
            agent.isStopped = true;
        }
    }
}
