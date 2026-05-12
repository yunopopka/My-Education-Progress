using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

// 1. То, что меняет игрок (или ИИ)
public struct VehicleInput : IComponentData
{
    public float Throttle; // от -1.0 (назад) до 1.0 (вперед)
    public float Steering; // от -1.0 (влево) до 1.0 (вправо)
}

// 2. Базовые характеристики (не меняются)
public struct VehicleStats : IComponentData
{
    public float Acceleration;    // Как быстро набирает скорость
    public float MaxSpeedRoad;    // Макс. скорость по асфальту
    public float MaxSpeedOffroad; // Макс. скорость по грязи
    public float TurnSpeed;       // Чувствительность руля
}

// 3. Текущее состояние машины
public struct VehicleState : IComponentData
{
    public float CurrentSpeed;
    public bool IsOnRoad;         // true = асфальт, false = грязь
}

public readonly partial struct VehicleAspect : IAspect
{
    private readonly RefRW<LocalTransform> transform;
    private readonly RefRW<VehicleState> state;
    private readonly RefRO<VehicleInput> input;
    private readonly RefRO<VehicleStats> stats;

    public void Drive(float dt)
    {
        // 1. Определяем лимит скорости (тернарный оператор)
        float maxSpeed = state.ValueRO.IsOnRoad ? stats.ValueRO.MaxSpeedRoad : stats.ValueRO.MaxSpeedOffroad;

        // 2. Считаем новую скорость и СРАЗУ ограничиваем её
        float newSpeed = state.ValueRO.CurrentSpeed + (input.ValueRO.Throttle * stats.ValueRO.Acceleration * dt);
        state.ValueRW.CurrentSpeed = math.clamp(newSpeed, -maxSpeed, maxSpeed);

        // 3. Двигаем машину ВПЕРЕД по её локальной оси
        transform.ValueRW.Position += transform.ValueRO.Forward() * state.ValueRO.CurrentSpeed * dt;

        // 4. Поворачиваем ТОЛЬКО если скорость больше 0.1
        // Используем math.abs (модуль числа), чтобы поворот работал и при езде назад
        if (math.abs(state.ValueRO.CurrentSpeed) > 0.1f)
        {
            // Небольшой хак: если едем назад, инвертируем руль для реализма (math.sign возвращает 1 или -1)
            float direction = math.sign(state.ValueRO.CurrentSpeed);

            transform.ValueRW.Rotation = math.mul(
                transform.ValueRO.Rotation,
                quaternion.RotateY(input.ValueRO.Steering * stats.ValueRO.TurnSpeed * direction * dt)
            );
        }
    }
}