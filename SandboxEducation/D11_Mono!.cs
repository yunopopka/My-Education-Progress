using System.Collections.Generic;
using UnityEngine;

public class LearningScript : MonoBehaviour
{
    
    public List<Transform> enemies = new List<Transform>();

    private void Update()
    {
        CleanRadar();
        FindClosestEnemy();
    }
    private Transform FindClosestEnemy()
    {
        Transform ClosestEnemy = null;
        float MinimumDistance = Mathf.Infinity;

        foreach(var unit in enemies)
        {
            if(unit == null) { continue; }
            else 
            { 
            float dist = Vector3.Distance(transform.position, unit.position);

            if(dist < MinimumDistance)
            {
                ClosestEnemy = unit;
                MinimumDistance = dist;
            }
        }
    }
        return ClosestEnemy;
    }

    private void CleanRadar()
    {
        for(int i = enemies.Count -1; i>=0 ; i--)
        {
            if (enemies[i] == null)
            {
                enemies.RemoveAt(i);
            }
        }
    }
}