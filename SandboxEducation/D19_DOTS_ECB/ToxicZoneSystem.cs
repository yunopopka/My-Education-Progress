using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct ToxicZoneSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()) { return; }
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        if (!SystemAPI.HasSingleton<ToxicZoneSingleton>()) { return; }
        var toxZone = SystemAPI.GetSingleton<ToxicZoneSingleton>();

        float dt = SystemAPI.Time.DeltaTime;

        foreach (var(uni,health,enti) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<_Health>>().WithEntityAccess())
        {
            float dist = math.distance(uni.ValueRO.Position, toxZone.Center);

            if(dist < toxZone.Radius) { health.ValueRW.Current -= toxZone.DamagePerSecond* dt; }

            if(health.ValueRO.Current <= 0) { ecb.DestroyEntity(enti); }
        }
    }
}
