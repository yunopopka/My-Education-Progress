using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine.LowLevelPhysics2D;
using Unity.VisualScripting;

public struct TankStats : IComponentData
{
    public float Acceleration;
    public float MaxSpeed;
    public float TurnSpeed;
    // Тот самый переключатель!
    // true = разворот на месте (НАТО). false = поворот только в движении (СССР)
    public bool CanPivotOnSpot;
}

public struct TankState : IComponentData
{
    public float CurrentSpeed;
}

public readonly partial struct TankAspect : IAspect
{
    private readonly RefRW<LocalTransform> transform;
    private readonly RefRW<TankState> state;
    private readonly RefRW<MoveCommand> command;
    private readonly RefRO<TankStats> stats;
    private readonly RefRO<TankSensors> sensors;

    private float3 CheckObstacles(ref PhysicsWorld physicsWorld)
    {
        // Немного приподнимаем стартовую позицию, чтобы не чиркать по земле
        float3 startCenter = transform.ValueRO.Position + new float3(0, 1.0f, 0);

        // Вектор "Вперед" на дистанцию луча
        float3 forwardOffset = transform.ValueRO.Forward() * sensors.ValueRO.RayDistance;

        // Вектор смещения "Вправо" на половину ширины танка
        float3 rightOffset = transform.ValueRO.Right() * (sensors.ValueRO.TankWidth / 2f);

        // 1. Луч по центру
        RaycastInput rayCenter = new RaycastInput
        {
            Start = startCenter,
            End = startCenter + forwardOffset,
            Filter = CollisionFilter.Default
        };

        // 2. Луч слева (отнимаем rightOffset)
        RaycastInput rayLeft = new RaycastInput
        {
            Start = startCenter - rightOffset,
            End = startCenter - rightOffset + forwardOffset,
            Filter = CollisionFilter.Default
        };

        // 3. Луч справа (прибавляем rightOffset)
        RaycastInput rayRight = new RaycastInput
        {
            Start = startCenter + rightOffset,
            End = startCenter + rightOffset + forwardOffset,
            Filter = CollisionFilter.Default
        };

        // Проверяем по очереди! Кто первый нашел стену — того и нормаль.
        if (physicsWorld.CastRay(rayCenter, out RaycastHit hitCenter)) return hitCenter.SurfaceNormal;
        if (physicsWorld.CastRay(rayLeft, out RaycastHit hitLeft)) return hitLeft.SurfaceNormal;
        if (physicsWorld.CastRay(rayRight, out RaycastHit hitRight)) return hitRight.SurfaceNormal;

        // Если все три луча прошли насквозь:
        return float3.zero;
    }

    public void Drive(float dt, ref PhysicsWorld physicsWorld)
    {
        if (!command.ValueRO.HasCommand) return;

        float3 _current = transform.ValueRO.Position;
        float3 _cmd = command.ValueRO.TargetPosition;
        float3 currentVector = _cmd - _current;
        currentVector.y = 0;

        float distance = math.length(currentVector);

        if (distance < 1.0f)
        {
            state.ValueRW.CurrentSpeed = 0;
            command.ValueRW.HasCommand = false;
            return;
        }

        // 1. Базовое направление прямо на цель
        float3 baseDirection = math.normalize(currentVector);

        // 2. Спрашиваем сенсоры: есть ли стена?
        float3 wallNormal = CheckObstacles(ref physicsWorld);
        float3 finalDirection = baseDirection;

        // Если wallNormal не равен нулю (math.lengthsq быстрее, чем math.length)
        if (math.lengthsq(wallNormal) > 0.1f)
        {
            // Отталкиваемся от стены!
            finalDirection = baseDirection + (wallNormal * sensors.ValueRO.AvoidanceForce);
            finalDirection.y = 0; // На всякий случай гасим высоту
            finalDirection = math.normalize(finalDirection);
        }

        // 3. Используем ИТОГОВОЕ направление для расчета поворота
        float dotForward = math.dot(transform.ValueRO.Forward(), finalDirection);
        float dotRight = math.dot(transform.ValueRO.Right(), finalDirection);

        float throttle = 0;
        float steering = math.sign(dotRight);

        float throttle = 0;
        float steering = math.sign(dotRight); // 1 (вправо) или -1 (влево)

        // --- ВЫБОР ДОКТРИНЫ ---
        if (stats.ValueRO.CanPivotOnSpot)
        {
            // НАТО: Если цель сильно сбоку, стоим и крутимся
            if (dotForward < 0.7f)
            {
                throttle = 0; // Газ не жмем
                // Крутим корпус на месте
                transform.ValueRW.Rotation = math.mul(transform.ValueRO.Rotation, quaternion.RotateY(stats.ValueRO.TurnSpeed * steering * dt));
            }
            else
            {
                throttle = 1; // Смотрим примерно на цель - жмем газ!
                // На ходу тоже чуть-чуть подруливаем
                transform.ValueRW.Rotation = math.mul(transform.ValueRO.Rotation, quaternion.RotateY(stats.ValueRO.TurnSpeed * steering * dt));
            }
        }
        else
        {
            // СССР: Всегда жмем газ, чтобы повернуть
            throttle = 1;

            // Крутим гусеницы ТОЛЬКО если есть скорость
            if (state.ValueRO.CurrentSpeed > 0.1f)
            {
                transform.ValueRW.Rotation = math.mul(transform.ValueRO.Rotation, quaternion.RotateY(stats.ValueRO.TurnSpeed * steering * dt));
            }
        }

        // --- ФИЗИКА ДВИЖЕНИЯ ---

        // Считаем скорость
        if (throttle > 0)
        {
            state.ValueRW.CurrentSpeed += stats.ValueRO.Acceleration * dt;
        }
        else
        {
            // Если газ отпущен (НАТО крутится), быстро тормозим
            state.ValueRW.CurrentSpeed -= stats.ValueRO.Acceleration * 2f * dt;
        }

        // Ограничиваем скорость: не меньше 0, не больше MaxSpeed
        state.ValueRW.CurrentSpeed = math.clamp(state.ValueRW.CurrentSpeed, 0, stats.ValueRO.MaxSpeed);

        // Двигаем танк вперед по локальной оси
        transform.ValueRW.Position += transform.ValueRO.Forward() * state.ValueRO.CurrentSpeed * dt;
    }
}