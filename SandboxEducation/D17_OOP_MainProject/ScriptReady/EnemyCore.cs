using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCore : MonoBehaviour
{
    public EnemyData data;
    private NavMeshAgent agent;
    private int _hp;
    public Action<int> death;
    private Transform Target;
    private int currentDamage;

    private float RaycastRange = 10f;
    private int currentHP
    {
        get { return _hp; }
        set
        {
            if (value <= 0)
            {
                _hp = 0;
                death?.Invoke(currentReward);
                gameObject.SetActive(false);
            }
            else _hp = value;
        }
    }
    private int currentReward;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        currentHP = data.Health;
        currentReward = data.Reward;
        agent.speed = data.Speed;
        currentDamage = data.Damage;
        if(Target != null) { agent.SetDestination(Target.position); }
    }
    private void Update() { if (agent.speed <= 2f) { Attack(); } }

    private void Attack()
    {
        RaycastHit hitInfo;

            if(Physics.Raycast(transform.position,transform.forward,out hitInfo, RaycastRange))
            {
            if(hitInfo.collider.TryGetComponent(out BuildingCore wall)) { wall.TakeDamage(currentDamage); }   
            }
    }
    public void TakeDamage(int dmg) { currentHP -= dmg; }
    public void TakeTarget(Transform trg) { Target = trg; }
}
