using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static event Action OnEnemyDied;
    public void Die()
    {
        OnEnemyDied?.Invoke();
    }
}
