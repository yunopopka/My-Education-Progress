using System.Linq;
using UnityEngine;

public class Aegis : MonoBehaviour
{
    public Transform[] allEnemies;

    public float scanRadius = 30f;
    public float turnSpeed = 5f;
    public GameObject missilePrefab;

    public Transform firePoint;
    public float fireRate = 1f;

    private Transform currentTarget;
    private float fireTimer;
    private float radarTimer;

    private Quaternion CurrentAngle;

    private void Start()
    {
        CurrentAngle = transform.rotation;
    }

    private void Update()
    {
        // 1. Медленный радар (оптимизация)
        radarTimer -= Time.deltaTime;
        if(radarTimer <= 0)
        {
            Scan();
            radarTimer = 0.5f; 
        }

        // 2. Стейт-машина (Стрелочник)
        if (currentTarget != null) 
        { 
            Attack(); 
        }
        else 
        { 
            // Возврат на базу, если нет цели
            transform.rotation = Quaternion.Slerp(transform.rotation, CurrentAngle, turnSpeed * Time.deltaTime); 
        }
    }

    private void Scan()
    {
        // Идеальный LINQ-запрос: Фильтруем -> Фильтруем -> Сортируем -> Берем
        currentTarget = allEnemies
            .Where(e => e != null) // Отсекаем призраков
            .Where(e => Vector3.Distance(transform.position, e.position) <= scanRadius) // Отсекаем далеких
            .OrderBy(e => Vector3.Distance(transform.position, e.position)) // Сортируем по близости
            .FirstOrDefault(); // Берем первого в списке
    }

    private void Attack()
    {
        // Наведение
        Vector3 directionToTarget = currentTarget.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

        // Стрельба
        fireTimer -= Time.deltaTime;

        if(fireTimer <= 0)
        {
            Instantiate(missilePrefab, firePoint.position, firePoint.rotation);
            fireTimer = fireRate;
        }

        // Защита от выхода из зоны поражения
        if(Vector3.Distance(transform.position, currentTarget.position) >= scanRadius) 
        { 
            currentTarget = null; 
        }
    }
}
