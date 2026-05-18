using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

// Тут полная генерация от ИИ, больше для меня что бы иметь шпаргалку
// 1. Приказ (вместо ручного ввода)
public struct MoveCommand : IComponentData
{
    public float3 TargetPosition;
    public bool HasCommand; // Есть ли куда ехать?
    public bool PreferRoads; // Для "Быстрого марша" (будет учитываться при глобальном поиске)
}

// 2. Характеристики (+ скорость заднего хода)
public struct VehicleStats : IComponentData
{
    public float Acceleration;
    public float MaxSpeedRoad;
    public float MaxSpeedOffroad;
    public float MaxReverseSpeed; // Лимит при езде назад
    public float TurnSpeed;
}

// 3. Состояние
public struct VehicleState : IComponentData
{
    public float CurrentSpeed;
    public bool IsOnRoad;
}

public readonly partial struct RTSVehicleAspect : IAspect
{
    private readonly RefRW<LocalTransform> transform;
    private readonly RefRW<VehicleState> state;
    private readonly RefRW<MoveCommand> command;
    private readonly RefRO<VehicleStats> stats;

    public void UpdateAIDriver(float dt)
    {
        if (!command.ValueRO.HasCommand) return; // Стоим, приказа нет

        // 1. Вектор к цели и дистанция
        float3 myPos = transform.ValueRO.Position;
        float3 targetPos = command.ValueRO.TargetPosition;
        float3 dirToTarget = targetPos - myPos;

        // Обнуляем Y, чтобы машина не пыталась улететь в небо или под землю
        dirToTarget.y = 0;

        float distance = math.length(dirToTarget);

        // 2. Проверка прибытия
        if (distance < 1.0f) // Радиус остановки
        {
            state.ValueRW.CurrentSpeed = 0; // Резко тормозим (в идеале нужно плавное торможение)
            command.ValueRW.HasCommand = false; // Приказ выполнен
            return;
        }

        // Нормализуем вектор для математики
        float3 normalizedDir = dirToTarget / distance;

        // 3. Анализ положения цели (dot product)
        float dotForward = math.dot(transform.ValueRO.Forward(), normalizedDir);
        float dotRight = math.dot(transform.ValueRO.Right(), normalizedDir);

        // Переменные, которые ИИ будет "нажимать"
        float throttle = 0;
        float steering = 0;

        // 4. СМАРТ-ЛОГИКА: Вперед или Назад?
        // Если цель сильно сзади (угол больше 120 градусов) И она близко (до 10 метров) -> сдаем назад
        if (dotForward < -0.5f && distance < 10f)
        {
            throttle = -1f; // Жмем реверс
            // При езде назад руль инвертируется, чтобы задняя часть повернула к цели
            steering = -math.sign(dotRight);
        }
        else
        {
            throttle = 1f; // Едем вперед
            // Поворачиваем в сторону цели (чем дальше цель сбоку, тем сильнее крутим руль)
            steering = math.clamp(dotRight * 2f, -1f, 1f);
        }

        // --- БАЗОВАЯ ЛОГИКА ДВИЖЕНИЯ (из прошлого урока) ---

        // Определяем лимит скорости в зависимости от направления и дороги
        float maxAllowedSpeed = throttle > 0
            ? (state.ValueRO.IsOnRoad ? stats.ValueRO.MaxSpeedRoad : stats.ValueRO.MaxSpeedOffroad)
            : stats.ValueRO.MaxReverseSpeed;

        // Ускорение (торможение при смене направления происходит естественно)
        float newSpeed = state.ValueRO.CurrentSpeed + (throttle * stats.ValueRO.Acceleration * dt);
        state.ValueRW.CurrentSpeed = math.clamp(newSpeed, -stats.ValueRO.MaxReverseSpeed, maxAllowedSpeed);

        // Движение вперед/назад
        transform.ValueRW.Position += transform.ValueRO.Forward() * state.ValueRO.CurrentSpeed * dt;

        // Поворот (только если двигаемся)
        if (math.abs(state.ValueRO.CurrentSpeed) > 0.1f)
        {
            float direction = math.sign(state.ValueRO.CurrentSpeed);
            transform.ValueRW.Rotation = math.mul(
                transform.ValueRO.Rotation,
                quaternion.RotateY(steering * stats.ValueRO.TurnSpeed * direction * dt)
            );
        }
    }
}
