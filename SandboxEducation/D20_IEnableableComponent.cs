using Unity.Entities;
using Unity.Burst;

public struct EMPCommand : IComponentData { }

public struct RobotEngine : IComponentData, IEnableableComponent
{
    public float Speed;
}

[BurstCompile]
public partial struct EMPTriggerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // 1. Проверь наличие EMPCommand
        if (!SystemAPI.HasSingleton<EMPCommand>()) { return; }

        // 2. Сделай цикл по всем роботам (нужна только Entity)
        foreach(var (engine,unit) in SystemAPI.Query<RobotEngine>().WithEntityAccess())
        {
            SystemAPI.SetComponentEnabled<RobotEngine>(unit, false);

        }
        state.EntityManager.DestroyEntity(SystemAPI.GetSingletonEntity<EMPCommand>());
    }
}