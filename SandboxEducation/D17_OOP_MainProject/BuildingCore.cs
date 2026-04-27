using UnityEngine;

public class BuildingCore : MonoBehaviour
{
    private int _hp;
    private int Cost;
    public TurretData data;
    private int Health
    {
        get { return _hp; }
        set
        {
            if (value <= 0)
            {
                _hp = 0;
                gameObject.SetActive(false);
            }
        }
    }

    private void Start() { Health = data.Health; Cost = data.Coast; }
    public void TakeDamage(int dmg) { Health -= dmg; }
}
