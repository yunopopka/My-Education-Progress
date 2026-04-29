using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
public struct NewWaypointCommand : IComponentData
{
    public bool IsNewClick;
    public float3 ClickPosition;
}

[BurstCompile]
public partial struct AddWaypointSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<NewWaypointCommand>()) { return; }
        var wayPoint = SystemAPI.GetSingleton<NewWaypointCommand>();
        if(!wayPoint.IsNewClick) { return; }


        if (!SystemAPI.HasSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()) { return; }
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach(var (uni,enty) in SystemAPI.Query<RefRO<UnitSelectionState>>().WithEntityAccess())
        {
            if(uni.ValueRO.IsSelected) { ecb.AppendToBuffer(enty, new WaypointElement { Position = wayPoint.ClickPosition }); }
        }

    }
}