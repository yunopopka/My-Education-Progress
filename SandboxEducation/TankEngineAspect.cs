using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

// 1. Приказ
public struct TankCommand : IComponentData
{
    public float3 TargetPosition;
    public bool HasCommand;
    public bool IsFastMarch; // Выжат ли "Форсаж" (игнорируем сохранение ресурса двигателя)
}

// 2. Характеристики двигателя и массы
public struct TankEngineStats : IComponentData
{
    public float BaseAcceleration; // Базовое ускорение
    public float BrakePower;       // Сила торможения
    public float MaxSpeed;
    public float TurnSpeed;
}

// 3. Текущее состояние (телеметрия)
public struct TankTelemetry : IComponentData
{
    public float CurrentSpeed;
    public float CurrentAcceleration; // Насколько сильно мы ускоряемся ПРЯМО СЕЙЧАС (нужно для наклона корпуса)
}

public readonly partial struct TankEngineAspect : IAspect
{
    private readonly RefRW<LocalTransform> transform;
    private readonly RefRW<TankTelemetry> telemetry;
    private readonly RefRW<TankCommand> command;
    private readonly RefRO<TankEngineStats> stats;

    public void UpdateEngine(float dt)
    {
        float oldSpeed = telemetry.ValueRO.CurrentSpeed;
        float3 Vect = command.ValueRO.TargetPosition - transform.ValueRO.Position;
        float distToTarg = math.length(Vect);

        float targetThrottle = 0;
        float targetBrake = 0;

        if(distToTarg > 5.0f) { targetThrottle = 1.0f; }
        if(distToTarg <= 5.0f && distToTarg > 0.5f) { targetThrottle = 0; targetBrake = 1.0f; }
        if(distToTarg <= 0.5f) { telemetry.ValueRW.CurrentSpeed = 0; command.ValueRW.HasCommand = false; return; }

        float torqueFactor = math.max(0.1f, 1.0f - (telemetry.ValueRO.CurrentSpeed / stats.ValueRO.MaxSpeed));

        float realTorque = stats.ValueRO.BaseAcceleration * torqueFactor * targetThrottle;
        if (command.ValueRO.IsFastMarch) { realTorque = realTorque * 1.5f; }

        telemetry.ValueRW.CurrentSpeed += realTorque * dt;

        if(targetBrake == 1.0f) { telemetry.ValueRW.CurrentSpeed -= stats.ValueRO.BrakePower * dt; }

        if (command.ValueRO.IsFastMarch) { telemetry.ValueRW.CurrentSpeed = math.clamp(telemetry.ValueRO.CurrentSpeed, 0, stats.ValueRO.MaxSpeed * 1.5f); }
        else { telemetry.ValueRW.CurrentSpeed = math.clamp(telemetry.ValueRO.CurrentSpeed, 0, stats.ValueRO.MaxSpeed); }

        telemetry.ValueRW.CurrentAcceleration = (telemetry.ValueRO.CurrentSpeed - oldSpeed) / dt;
    }
}