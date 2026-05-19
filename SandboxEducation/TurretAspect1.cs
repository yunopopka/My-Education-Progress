using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct TurretData : IComponentData
{
    public Entity ShellPrefab; // Что спавним
    public float BulletSpeed;  // С какой скоростью полетит
    public float FireRate;     // Задержка между выстрелами
    public float ReloadTimer;  // Текущий таймер перезарядки
}

// Эту структуру мы будем собирать и передавать извне (от радара)
public struct TargetInfo
{
    public float3 Position;
    public float3 Velocity; // Куда и как быстро едет враг
    public bool IsValid;    // Есть ли вообще цель
}

public readonly partial struct TurretAspect : IAspect
{
    private readonly RefRW<LocalTransform> transform;
    private readonly RefRW<TurretData> turret;

    public void AimAndShoot(float dt, TargetInfo target, EntityCommandBuffer.ParallelWriter ecb, int sortKey)
    {
        turret.ValueRW.ReloadTimer -= dt;

        if (turret.ValueRO.ReloadTimer > 0 || !target.IsValid) return;

        float dist = math.distance(transform.ValueRO.Position, target.Position);

        float FlyTime = dist / turret.ValueRO.BulletSpeed;

        float3 futurePoint = target.Position + (target.Velocity * FlyTime);

        float3 currentVector = math.normalize(futurePoint - transform.ValueRO.Position);

        Entity enty = ecb.Instantiate(sortKey, turret.ValueRO.ShellPrefab);

        float3 spawnPos = transform.ValueRO.Position + (math.up() * 1.5f) + (currentVector * 2.0f);
        ecb.SetComponent(sortKey, enty, LocalTransform.FromPosition(spawnPos));

        ecb.SetComponent(sortKey, enty, new ArtilleryShell
        {
            Velocity = currentVector * turret.ValueRO.BulletSpeed,
            Gravity = 9.8f,
            LifeTime = 5f
        });

        turret.ValueRW.ReloadTimer = turret.ValueRO.FireRate;
    }
}