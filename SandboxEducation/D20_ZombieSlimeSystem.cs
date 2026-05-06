using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;

public struct ZombieDropper : IComponentData
{
    public Entity SlimePrefab;  // Чертеж лужи
    public float Timer;         // Таймер
    public float DropInterval;  // Интервал (например, каждые 3 секунды)
}

[BurstCompile]
public partial struct ZombieSlimeSystem : ISystem
{
    [BurstCompile]
    public partial struct DropSlimeJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;

        void Execute([EntityIndexInQuery] int sortKey,in LocalTransform transform,ref ZombieDropper dropper)
        {
            dropper.Timer -= DeltaTime;

            if(dropper.Timer <= 0)
            {
                Entity newSmile = ECB.Instantiate(sortKey,dropper.SlimePrefab);
                ECB.SetComponent(sortKey, newSmile, LocalTransform.FromPosition(transform.Position));
                dropper.Timer = dropper.DropInterval;
            }
        }
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        DropSlimeJob newjob = new DropSlimeJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            ECB = ecb.AsParallelWriter()
        };
        newjob.ScheduleParallel();
    }
}