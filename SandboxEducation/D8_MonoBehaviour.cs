using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

[BurstCompile]
public partial struct HomingSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        // Нам нужен LocalTransform на запись (чтобы двигать ракету)
        // И HomingMissile на чтение (чтобы знать куда и как быстро лететь)

        foreach (var (transform, missile) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<HomingMissile>>())
        {
            // 1. Получи текущую позицию ракеты из transform.ValueRO.Position
            // 2. Получи позицию цели из missile.ValueRO.TargetPosition
            // 3. Вычисли вектор направления (Цель - Текущая позиция) и нормализуй его через math.normalize()
            // 4. Прибавь к текущей позиции transform.ValueRW.Position вычисленный шаг (направление * скорость * dt)

            // ТВОЙ КОД ЗДЕСЬ:

            float3 CurrentPos = missile.ValueRO.TargetPosition - transform.ValueRO.Position;

            float3 normalizedDirection = math.normalize(CurrentPos);

            transform.ValueRW.Position += normalizedDirection * missile.ValueRO.Speed * dt;

            // Записать в 1 строку звучит как вызов - принимаю

            transform.ValueRW.Position += math.normalize(missile.ValueRO.TargetPosition - transform.ValueRO.Position) * missile.ValueRO.Speed * dt;
        }
    }
}