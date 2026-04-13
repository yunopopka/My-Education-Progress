using System.Collections.Generic;
using UnityEngine;

public class LearningScript : MonoBehaviour
{
    private Queue<Vector3> waypoints = new Queue<Vector3>();

    private Vector3 currentTargetPoint;
    private bool isMoving = false;

    private void Update()
    {
        Patrol();
    }

    public void AddWaypoint(Vector3 point)
    {
        waypoints.Enqueue(point);
    }

    private void Patrol()
    {
        if(!isMoving && waypoints.Count > 0)
        {
            currentTargetPoint = waypoints.Dequeue();
            isMoving = true;
        }
        
        if(isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentTargetPoint, 5f * Time.deltaTime);

            if (Vector3.Distance(transform.position, currentTargetPoint) < 0.1f)
            {
                isMoving = false;
            }

        }
    }

}