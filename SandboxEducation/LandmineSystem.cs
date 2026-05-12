using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

public struct Landmine : IComponentData
{
    public Entity ExplosionPrefab; // Префаб эффекта взрыва
    public float Timer;            // Таймер до взрыва
}

[BurstCompile]
public partial struct MineJob : IJobEntity
{
    public float DeltaTime;

    public EntityCommandBuffer.ParallelWriter ECB;

    void Execute([EntityIndexInQuery] int sortKey, Entity entity, ref Landmine mine, in LocalTransform transform)
    {
        mine.Timer -= DeltaTime;
        if (mine.Timer <= 0)
        {
            Entity enty = ECB.Instantiate(sortKey, mine.ExplosionPrefab);

            ECB.SetComponent(sortKey,enty, LocalTransform.FromPosition(transform.Position));

            ECB.DestroyEntity(sortKey, entity);
        }
    }
}

[BurstCompile]
public partial struct LandmineSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        MineJob newJob = new MineJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            ECB = ecb.AsParallelWriter()
        };

        newJob.ScheduleParallel();
    }
}