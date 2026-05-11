using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    private int KillCount = 0;

    private void OnEnable()
    {
        Enemy.OnEnemyDied += UpdateKillCount;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyDied -= UpdateKillCount;
    }

    private void UpdateKillCount()
    {
        KillCount += 1;
        if(KillCount == 3) { Debug.Log("Goblin groza"); }
    }
}
