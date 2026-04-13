using System.Linq;
using UnityEngine;

public class IronDome : MonoBehaviour
{
    public Transform[] enemies;
    private Transform? CurrentTarget;

    public float scanDist = 50f;
    public float TurretSpeed = 5f;
    public float ReloadSpeed = 2f;
    public float ScanRad = 0.5f;


    public GameObject ProjectilePrefab;
    public Transform Turret;
    public Transform Gun;

    Quaternion currentAngle;

    private float ScanTimer = 1f;
    private float AttackTimer = 1f;

    private void Start()
    {
        currentAngle = Turret.rotation;
    }

    private void Update()
    {
        ScanTimer -= Time.deltaTime;
        if(ScanTimer <= 0)
        {
            Scan();
            ScanTimer = ScanRad;
        }

        if(CurrentTarget != null)
        {
            Attack();
        }
        else
        {
            Sleep();
        }
    }

    private void Sleep()
    {
        Turret.rotation = Quaternion.Slerp(Turret.rotation, currentAngle,TurretSpeed * Time.deltaTime);
    }

    private void Scan()
    {
        CurrentTarget = enemies.Where(e => e != null)
                               .Where(e => Vector3.Distance(transform.position, e.position) <= scanDist)
                               .OrderBy(e => Vector3.Distance(transform.position, e.position)) // transform ибо считаю дистанцию от тела турели
                               .FirstOrDefault();
    }

    private void Attack()
    {
        Vector3 currentAngle = CurrentTarget.position - Turret.position;
        Quaternion AngleRotate = Quaternion.LookRotation(currentAngle);

        Turret.rotation = Quaternion.Slerp(Turret.rotation, AngleRotate, TurretSpeed * Time.deltaTime);

        RaycastHit hitInfo;

        if(Physics.Raycast(Gun.position, Gun.forward, out hitInfo, scanDist))
        {
            if (hitInfo.collider.CompareTag("Enemy"))
            {
                AttackTimer -= Time.deltaTime;
            }
            else
            {
                AttackTimer = ReloadSpeed;
            }

        }

        if (AttackTimer <= 0)
        {
            Instantiate(ProjectilePrefab, Gun.position, Gun.rotation);
            AttackTimer = ReloadSpeed;
        }
    }

}

