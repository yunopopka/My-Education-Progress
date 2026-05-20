using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;

// Состояния полета
public enum HeliFlightState
{
    Landed,       // На земле, винты стоят
    SpoolingUp,   // Раскручиваем винты
    Flying,       // В воздухе (полный контроль)
    Landing,      // Снижаемся для посадки
    SpoolingDown  // Остываем, глушим винты
}

// Тумблер высоты
public enum HeliAltitude { Low, High }

// 1. Приказы
public struct HeliCommand : IComponentData
{
    public float3 TargetPosition;
    public HeliAltitude DesiredAltitude; // Низко или Высоко
    public bool OrderLand;               // Игрок приказал сесть (true)
}

// 2. Характеристики
public struct HeliStats : IComponentData
{
    public float LowHeight;         // Например, 10 метров
    public float HighHeight;        // Например, 30 метров
    public float VerticalSpeed;     // Как быстро набирает высоту (м/с)
    public float RotorSpoolSpeed;   // Скорость разгона винтов
    public float Acceleration;  // Сила тяги вперед
    public float MaxSpeed;      // Максимальная скорость
    public float Drag;          // Сопротивление воздуха (торможение)
    public float MaxPitchAngle; // Максимальный наклон носа (в радианах)
    public float TurnSpeed;     // Скорость поворота корпуса
}

// 3. Телеметрия (Текущее состояние)
public struct HeliTelemetry : IComponentData
{
    public HeliFlightState State;
    public float RotorRPM; // От 0.0 (стоят) до 1.0 (максимум)

    // Эту переменную мы будем использовать на Этапе 2 для инерции, 
    // но объявить её нужно уже сейчас.
    public float3 CurrentVelocity;
}

public readonly partial struct HeliVerticalAspect : IAspect
{
    private readonly RefRW<LocalTransform> transform;
    private readonly RefRW<HeliTelemetry> telemetry;
    private readonly RefRO<HeliCommand> command;
    private readonly RefRO<HeliStats> stats;

    public void UpdateVertical(float dt)
    {
        // Для удобства чтения/записи
        HeliFlightState currentState = telemetry.ValueRO.State;
        float currentRPM = telemetry.ValueRO.RotorRPM;
        float3 pos = transform.ValueRO.Position;

        switch (currentState)
        {
            case HeliFlightState.Landed:
                if (!command.ValueRO.OrderLand) { currentState = HeliFlightState.SpoolingUp; }
                break;
            case HeliFlightState.SpoolingUp:
                currentRPM = math.saturate(currentRPM + stats.ValueRO.RotorSpoolSpeed * dt);
                if (currentRPM >= 1.0f) { currentState = HeliFlightState.Flying; }
                break;
            case HeliFlightState.Flying:
                if (command.ValueRO.OrderLand) { currentState = HeliFlightState.Landing; }
                else
                { 
                    float targetY = command.ValueRO.DesiredAltitude == HeliAltitude.Low
                        ? stats.ValueRO.LowHeight
                        : stats.ValueRO.HighHeight;
                    pos.y = math.lerp(pos.y, targetY, stats.ValueRO.VerticalSpeed * dt);
                }
                break;
            case HeliFlightState.Landing:
                pos.y = math.max(0f, pos.y - (stats.ValueRO.VerticalSpeed * dt));
                if (pos.y <= 0.1f) { pos.y = 0; currentState = HeliFlightState.SpoolingDown; }
                break;
            case HeliFlightState.SpoolingDown:
                currentRPM = math.saturate(currentRPM - stats.ValueRO.RotorSpoolSpeed * dt);
                if (currentRPM <= 0.0f) { currentState = HeliFlightState.Landed; }
                break;
        }

        telemetry.ValueRW.State = currentState;
        telemetry.ValueRW.RotorRPM = currentRPM;
        transform.ValueRW.Position = pos;
    }
}
public readonly partial struct HeliHorizontalAspect : IAspect
{
    private readonly RefRW<LocalTransform> transform;
    private readonly RefRW<HeliTelemetry> telemetry;
    private readonly RefRO<HeliCommand> command;
    private readonly RefRO<HeliStats> stats;

    public void UpdateHorizontal(float dt)
    {
        float3 pos = transform.ValueRO.Position;
        float3 velocity = telemetry.ValueRO.CurrentVelocity;
        quaternion rot = transform.ValueRO.Rotation; // Добавили переменную для вращения

        // 1. Если не летим - просто тормозим (гасим инерцию)
        if (telemetry.ValueRO.State != HeliFlightState.Flying)
        {
            velocity = math.lerp(velocity, float3.zero, stats.ValueRO.Drag * dt);
            pos += velocity * dt; // Не забываем применить остаточную скорость!

            transform.ValueRW.Position = pos;
            telemetry.ValueRW.CurrentVelocity = velocity;
            return;
        }

        // 2. Поиск цели
        float3 offsetToTarget = command.ValueRO.TargetPosition - pos;
        offsetToTarget.y = 0; // Игнорируем высоту
        float dist = math.length(offsetToTarget);

        // 3. Ускорение (Газ)
        if (dist > 1.0f)
        {
            float3 dir = math.normalize(offsetToTarget);
            velocity += dir * stats.ValueRO.Acceleration * dt;
        }

        // 4. Торможение об воздух (работает ВСЕГДА)
        velocity = math.lerp(velocity, float3.zero, stats.ValueRO.Drag * dt);

        // 5. Ограничение максимальной скорости
        if (math.length(velocity) > stats.ValueRO.MaxSpeed)
        {
            velocity = math.normalize(velocity) * stats.ValueRO.MaxSpeed;
        }

        // 6. Движение
        pos += velocity * dt;

        // 7. Вращение по вектору движения
        if (math.length(velocity) > 0.1f)
        {
            // Создаем кватернион, который смотрит туда, куда мы летим
            quaternion targetRotation = quaternion.LookRotationSafe(math.normalize(velocity), math.up());

            // Плавно поворачиваемся к нему (slerp - это lerp для кватернионов)
            rot = math.slerp(rot, targetRotation, stats.ValueRO.TurnSpeed * dt);
        }

        // 8. Запись результатов
        transform.ValueRW.Position = pos;
        transform.ValueRW.Rotation = rot;
        telemetry.ValueRW.CurrentVelocity = velocity;
    }
}