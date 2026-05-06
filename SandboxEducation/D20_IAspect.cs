using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;
public struct Fuel : IComponentData
{
    public float Amount;
    public float BurnRate; // Сколько сжигаем в секунду
}

public struct Speed : IComponentData
{
    public float Value;
}

public readonly partial struct SpaceshipAspect : IAspect
{
    private readonly RefRW<Fuel> _fuel;
    private readonly RefRO<Speed> _speed;
    private readonly RefRW<LocalTransform> _transform;

    public void Fly(float dt)
    {
        if(_fuel.ValueRO.Amount > 0)
        {
            _transform.ValueRW.Position += _transform.ValueRO.Forward() * _speed.ValueRO.Value * dt;
            _fuel.ValueRW.Amount -= _fuel.ValueRO.BurnRate * dt;
        }
    }
}

[BurstCompile]
public partial struct SpaceshipSystem : ISystem
{
    [BurstCompile]
    public partial struct FlyJob : IJobEntity
    {
        public float DeltaTime;

        void Execute(SpaceshipAspect aspect)
        {
            aspect.Fly(DeltaTime);
        }
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new FlyJob { DeltaTime = SystemAPI.Time.DeltaTime }.ScheduleParallel();
    }
}