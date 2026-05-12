using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct Turret : IComponentData
{
    public float ViewRadius; // Радиус поиска
    public Entity CurrentTarget; // Сюда запишем ID найденного врага
}
// У турели есть LocalTransform
// У врага есть EnemyTag и LocalTransform

public readonly partial struct TurretAspect : IAspect
{
    private readonly RefRW<Turret> turret;
    private readonly RefRO<LocalTransform> transform;

    // Псевдо-метод. В реальности мы бы передали сюда данные из Системы
    public void FindTarget(DynamicBuffer<EnemyData> allEnemies)
    {
        float closestDistance = float.MaxValue; // Стартуем с бесконечности
        Entity bestTarget = Entity.Null;        // Entity.Null означает "никого нет"

        float3 myPos = transform.ValueRO.Position;


        for(int i = 0; i < allEnemies.Length; i++)
        {
            float3 EnemyPos = allEnemies[i].Position;

            float dist = math.distance(myPos, EnemyPos);
            if(dist < turret.ValueRO.ViewRadius && dist < closestDistance)
            {
                closestDistance = dist;
                bestTarget = allEnemies[i].EnemyEntity;
            }
        }
        turret.ValueRW.CurrentTarget = bestTarget;
    }
}

// Структура для массива
public struct EnemyData : IBufferElementData
{
    public Entity EnemyEntity;
    public float3 Position;
}