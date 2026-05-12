using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
public partial struct PlayerHealSystem : ISystem
{
    private void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        foreach (var (health, tag) in SystemAPI.Query<RefRW<Health>, RefRO<PlayerTag>>())
        {
            health.ValueRW.Current += 1.0f * dt;
            health.ValueRW.Current = math.min(health.ValueRO.Current, health.ValueRO.Max);
        }
    }
}
