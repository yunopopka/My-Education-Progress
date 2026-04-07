using System.Linq;
using UnityEngine;

public class MedicDrone : MonoBehaviour
{
    public TankHealth[] alliedTanks;
    TankHealth TargetToHeal;
    private float HealTimer;

    private void Update()
    {
        HealTimer -= Time.deltaTime;
        if(HealTimer <= 0)
        {
            FindTargetToHeal();
            HealTimer = 1f;

            if (TargetToHeal != null)
            {
                Debug.Log("Отправлен бот к танку! У него осталось " + TargetToHeal.currentHp + " ХП");
            }
        }
    }

    private void FindTargetToHeal()
    {
        TargetToHeal = alliedTanks.Where(e => e != null)
                                            .Where(e => e.currentHp < 100)
                                            .OrderBy(e => e.currentHp)
                                            .FirstOrDefault();
    }
}
