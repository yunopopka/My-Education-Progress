using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;


[BurstCompile]
public partial struct SelectionSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        if (!SystemAPI.HasSingleton<SelectionInputData>()) { return; }

        var select = SystemAPI.GetSingleton<SelectionInputData>();

        if(!select.IsNewClick) { return; }

        foreach(var (unit,stateUni) in SystemAPI.Query<RefRO<LocalTransform>,RefRW<UnitSelectionState>>())
        {
            float dist = math.distance(unit.ValueRO.Position, select.ClickPosition);

            stateUni.ValueRW.IsSelected = (dist <= select.Radius);
        }
    }
}