using UnityEngine;

[CreateAssetMenu(fileName = "TurretData", menuName = "Scriptable Objects/TurretData")]
public class TurretData : ScriptableObject
{
    public int ReloadTime;
    public int Damage;
    public int Coast;
    public int TurretSpeed;
    public int Health;
}
