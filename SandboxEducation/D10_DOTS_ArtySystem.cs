using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Transforms;

// Синглтон с командой от инпута
public struct ArtilleryStrikeInput : IComponentData
{
    public bool IsStrikeActive; // Бахнуло ли в этом кадре?
    public float3 StrikeCenter; // Эпицентр
    public float Radius;        // Радиус поражения
    public float Damage;        // Урон
}

[BurstCompile]
public partial struct ArtillerySystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<ArtilleryStrikeInput>()) { return; }

        var unit = SystemAPI.GetSingleton<ArtilleryStrikeInput>();

        if(!unit.IsStrikeActive) { return; }

        foreach(var(Arty,target) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<Health>>())
        {
            float dist = math.distance(Arty.ValueRO.Position,unit.StrikeCenter);

            if(dist <= unit.Radius) { target.ValueRW.Current -= unit.Damage; }
        }
    }
}