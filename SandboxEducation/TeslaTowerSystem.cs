using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

public struct TeslaTower : IComponentData
{
    public Entity LightningPrefab; // Чертеж молнии
    public float Timer;            // Текущее время до выстрела
    public float FireRate;         // Интервал выстрела (например, 2 секунды)
}
[BurstCompile]
public partial struct TeslaTowerSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach(var (turret,transform) in SystemAPI.Query<RefRW<TeslaTower>, RefRO<LocalTransform>>())
        {
            turret.ValueRW.Timer -= dt;
            if(turret.ValueRO.Timer <= 0)
            {
                Entity entyclone = ecb.Instantiate(turret.ValueRO.LightningPrefab);

                ecb.SetComponent(entyclone, LocalTransform.FromPosition(transform.ValueRO.Position));

                turret.ValueRW.Timer = turret.ValueRO.FireRate;
            }
        }
    }
}
