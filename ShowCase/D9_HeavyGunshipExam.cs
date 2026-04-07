// ИИ чуток  поправил но на 90% логика написана мною, споткнулся в парочке ошибок но ничего, исправим

using UnityEngine;

public class HeavyGunship : MonoBehaviour
{
    public Transform player;
    public Transform guardPoint;

    public float chaseRadius = 20f;
    public float attackRadius = 10f;

    public float moveSpeed = 8f;
    public float turnSpeed = 4f;

    public GameObject misslePrefab;
    public Transform firePoint;

    public float fireRate = 1.5f;

    private float fireTimer;
    private bool IsChasing = false;
    private bool IsAttack = false;

    private void Update()
    {
        // 1. МОЗГ: Сначала анализируем обстановку и ставим флажки
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > chaseRadius)
        {
            IsChasing = false;
            IsAttack = false;
        }
        else if (dist > attackRadius && dist <= chaseRadius) // Добавил <= для точности
        {
            IsChasing = true;
            IsAttack = false;
        }
        else // dist <= 10
        {
            IsChasing = true;
            IsAttack = true;
        }

        // 2. МЫШЦЫ: Действуем согласно флажкам
        GuardAndChase();

        if (IsAttack == true) // Исправление бага бомбардировщика
        {
            Attack();
        }
    }

    private void GuardAndChase()
    {
        Vector3 CurrentPosition;

        // Выбираем цель
        if (IsChasing == false)
        {
            CurrentPosition = guardPoint.position;
        }
        else
        {
            CurrentPosition = player.position;
        }

        // Движение (только если не атакуем)
        if (IsAttack == false)
        {
            transform.position = Vector3.MoveTowards(transform.position, CurrentPosition, moveSpeed * Time.deltaTime);
        }

        // Поворот
        Vector3 PointVector = CurrentPosition - transform.position;

        // Защита от нулевого вектора (если долетели до базы)
        if (PointVector != Vector3.zero)
        {
            Quaternion CurrentVector = Quaternion.LookRotation(PointVector);
            transform.rotation = Quaternion.Slerp(transform.rotation, CurrentVector, turnSpeed * Time.deltaTime);
        }
    }

    private void Attack()
    {
        fireTimer -= Time.deltaTime;

        if (fireTimer <= 0)
        {
            Instantiate(misslePrefab, firePoint.position, firePoint.rotation);
            fireTimer = fireRate;
        }
    }
}