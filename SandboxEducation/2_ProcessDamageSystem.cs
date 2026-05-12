using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

// Элемент массива
public struct DamageBufferElement : IBufferElementData
{
    public float DamageValue;
}

[BurstCompile]
public partial struct DamageJob : IJobEntity
{
    public float DeltaTime;

    void Execute(DamageAspect aspect)
    {
        aspect.TakeDamageFromBuffer();
    }
}
[BurstCompile]
public partial struct ProcessDamageSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        DamageJob newJob = new DamageJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        };

        newJob.ScheduleParallel();
    }
}