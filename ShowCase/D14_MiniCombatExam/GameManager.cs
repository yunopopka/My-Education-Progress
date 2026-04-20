using System;
using System.Collections;
using UnityEngine;

public class GameManage : MonoBehaviour
{
    public Transform Target;
    public Transform SpawnPoint;

    public EnemyPool myPool;

    public WaveConfig[] configCurrent;// массив для загрузки нескольких карточек и дальнейшего перебора 1 - 3 волна
    private int CurrentWave = 0;

    private bool isSpawning = false;

    public void WaveButton()
    {

        if (!isSpawning)
        {

            Debug.Log("Current Wave: " + (CurrentWave + 1));
            StartCoroutine(StartWave());
        }
    }

    private void FixedUpdate()
    {
        CurrentWaveNumber();
    }

    private void CurrentWaveNumber()
    {
        if(CurrentWave == 3)
        {
            CurrentWave = 0;
        }
    }

    private IEnumerator StartWave()
    {
        isSpawning = true;

        for(int i = 0; i < configCurrent[CurrentWave].EnemyCount; i++)
        {
            yield return new WaitForSeconds(configCurrent[CurrentWave].TimeDelay);

            GameObject enemy = myPool.GetPooledEnemy();

            if (enemy != null)
            {
                enemy.transform.position = SpawnPoint.position;
                enemy.transform.rotation = SpawnPoint.rotation;
                if (enemy.TryGetComponent(out AiUnit comp))
                {
                    comp.TakeEnemy(Target);
                }

                enemy.SetActive(true);
            }
        }
        CurrentWave++;
        isSpawning = false;
    }


}
