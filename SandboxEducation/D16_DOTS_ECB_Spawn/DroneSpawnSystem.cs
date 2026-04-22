using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;

[BurstCompile]
public partial struct DroneSpawnSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()) return;
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        float dt = SystemAPI.Time.DeltaTime;

        foreach (var (drone, spawn) in SystemAPI.Query<RefRW<DroneSpawner>, RefRO<LocalTransform>>())
        {
            drone.ValueRW.Timer -= dt;


            if (drone.ValueRO.Timer <= 0)
            {
                
                Entity x = ecb.Instantiate(drone.ValueRO.DronePrefab);
                ecb.SetComponent(x, LocalTransform.FromPosition(spawn.ValueRO.Position));


                drone.ValueRW.Timer = drone.ValueRO.SpawnInterval;
            }
        }
    }
}