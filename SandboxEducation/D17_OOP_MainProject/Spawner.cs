using System.Collections;
using System.Linq;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform SpawnPoint;
    public _EnemyPool pool;
    public Transform Target;
    public WaveConfig[] waveConfig;

    private int CurrentWave = 0;
    private bool isSpawning = false;

    public void ButtonStartGame()
    {
        if (!isSpawning)
        {
            StartCoroutine(StartBattle());
        }
    }

    private IEnumerator StartBattle()
    {
        isSpawning = true;
        for (int i = 0; i < waveConfig.Length; i++) 
        { 
        for (int j = 0; j < waveConfig[CurrentWave].EnemyCount; j++)
        {
            yield return new WaitForSeconds(waveConfig[CurrentWave].TimeDelay);

            GameObject enemy = pool.GetPooledEnemy();

            if (enemy != null)
            {
                enemy.transform.position = SpawnPoint.position;
                enemy.transform.rotation = SpawnPoint.rotation;
                if (enemy.TryGetComponent(out EnemyCore unit))
                {
                    unit.TakeTarget(Target);
                }
                enemy.SetActive(true);
            }
        }
        CurrentWave++;
        if(CurrentWave < waveConfig.Length) { yield return new WaitForSeconds(waveConfig[CurrentWave].WaveDelay); }
        else { isSpawning = false; CurrentWave = 0;  Debug.Log("Attack is end"); }
        }
    }
}
