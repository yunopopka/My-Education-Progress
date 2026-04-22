using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;

[BurstCompile]
public partial struct DroneCleanupSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()) return;
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        float dt = SystemAPI.Time.DeltaTime;

        foreach (var (drone, entity) in SystemAPI.Query<RefRW<DroneLifetime>>().WithEntityAccess())
        {
            drone.ValueRW.TimeLeft -= dt;

            if(drone.ValueRO.TimeLeft <= 0)
            {
                ecb.DestroyEntity(entity);
            }
        }
    }
}