using UnityEngine;


public class LearningScript : MonoBehaviour
{
    public GameObject dronePrefab;
    public Transform spawnPoint;
    public float spawnInterval = 3f;
    public int maxDrones = 5;

    private int spawnedCount = 0;

    private void Update()
    {
     if(spawnedCount < maxDrones) { Spawn(); }   
    }

    private void Spawn()
    {
        spawnInterval -= Time.deltaTime;

        if(spawnInterval <= 0)
        {
            Instantiate(dronePrefab, spawnPoint.position, spawnPoint.rotation);
            spawnedCount++;
            spawnInterval = 3f;
            Debug.Log("Drone spawned! Drones count: " + spawnedCount);
        }
    }

}
