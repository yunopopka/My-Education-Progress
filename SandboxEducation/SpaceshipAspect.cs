using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

// 1. Приказ (куда лететь)
public struct ShipCommand : IComponentData
{
    public float3 TargetPosition;
    public bool HasCommand;
}

// 2. Характеристики двигателя
public struct ShipStats : IComponentData
{
    public float Acceleration; // Сила двигателя
    public float MaxSpeed;     // Лимит скорости
}

// 3. Текущее состояние (инерция)
public struct ShipState : IComponentData
{
    public float3 Velocity; // Вектор скорости (куда и как быстро летим сейчас)
}

public readonly partial struct SpaceshipAspect : IAspect
{
    private readonly RefRW<LocalTransform> transform;
    private readonly RefRW<ShipState> state;
    private readonly RefRO<ShipCommand> command;
    private readonly RefRO<ShipStats> stats;

    public void Fly(float dt)
    {
        if (!command.ValueRO.HasCommand) return;

        float3 pos = transform.ValueRO.Position;
        float3 vel = state.ValueRO.Velocity;
        float3 target = command.ValueRO.TargetPosition;

        float3 FloatToTarget = target - pos;
        float3 dir = math.normalize(FloatToTarget);

        vel += dir * stats.ValueRO.Acceleration * dt;

        if(math.length(vel) > stats.ValueRO.MaxSpeed) { vel = math.normalize(vel) * stats.ValueRO.MaxSpeed; }

        pos += vel * dt;

        transform.ValueRW.Position = pos;
        state.ValueRW.Velocity = vel;

    }
}