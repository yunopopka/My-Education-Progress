using UnityEngine;

[CreateAssetMenu(fileName = "WaveConfig", menuName = "Scriptable Objects/WaveConfig")]
public class WaveConfig : ScriptableObject
{
    public int EnemyCount;
    public float TimeDelay;
    public float WaveDelay;
}
