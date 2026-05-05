using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;

public struct Health : IComponentData
{
    public float Current;
    public float Max;
}

public struct Regeneration : IComponentData
{
    public float HealPerSecond;
}

[BurstCompile]
public partial struct RegenerationSystem : ISystem
{
    // ТВОЙ JOB:
    [BurstCompile]
    public partial struct RegenJob : IJobEntity
    {
        public float DeltaTime;

        void Execute(ref Health hp, in Regeneration reg)
        {
            hp.currentHP += reg.HealPerSecond * DeltaTime;
            if(hp.currentHP >= hp.Max) { hp.currentHP = hp.Max; }
        }
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        RegenJob newJob = new RegenJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        };

        newJob.ScheduleParallel();

    }
}