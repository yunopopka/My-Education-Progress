using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Transforms;

// 1. Вешаем на каждого юнита, которого можно выделить
public struct SelectableUnit : IComponentData
{
    public bool IsSelected;
}

// 2. Этот компонент будет висеть на одной уникальной сущности (Singleton)
public struct SelectionCommand : IComponentData
{
    public float3 MinPoint; // Нижний левый угол рамки (в 3D пространстве)
    public float3 MaxPoint; // Верхний правый угол рамки (в 3D пространстве)
    public bool TriggerSelection; // true = игрок отпустил кнопку мыши, пора выделять!
}

[BurstCompile]
public partial struct SelectionJob : IJobEntity
{
    public float3 Min;
    public float3 Max;

    // Джоба бежит только по тем, у кого есть SelectableUnit и LocalTransform
    void Execute(ref SelectableUnit unit, in LocalTransform transform)
    {
        bool inX = transform.Position.x >= Min.x && transform.Position.x <= Max.x;

        // 2. Проверяем попадание по оси Z (Глубина / Длина)
        bool inZ = transform.Position.z >= Min.z && transform.Position.z <= Max.z;

        // 3. Записываем результат напрямую! 
        // Если inX и inZ равны true, запишется true. Если хотя бы одно false -> запишется false.
        unit.IsSelected = inX && inZ;
    }
}

[BurstCompile]
public partial struct SelectionSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Ищем наш синглтон-приказ (проверяем, есть ли он вообще на сцене)
        if (!SystemAPI.TryGetSingleton<SelectionCommand>(out var command)) return;

        // Если приказа выделять нет - система спит
        if (!command.TriggerSelection) return;

        // Настраиваем Джобу
        SelectionJob job = new SelectionJob
        {
            Min = command.MinPoint,
            Max = command.MaxPoint
        };

        // Запускаем на всех ядрах!
        job.ScheduleParallel();

        // Сбрасываем триггер, чтобы не выделять одно и то же каждый кадр (в реальности это делается через ECB)
        // Для простоты примера мы это пропустим.
    }
}