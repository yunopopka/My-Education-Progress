using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;

public struct WaypointElement : IBufferElementData
{
    public float3 Position;
}

[BurstCompile]
public partial struct WaypointSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        
        foreach (var (transform, buffer) in SystemAPI.Query<RefRW<LocalTransform>, DynamicBuffer<WaypointElement>>())
        {
            
            if(buffer.Length == 0) { continue; }

            float3 currentTarget = buffer[0].Position;

            float dist = math.distance(transform.ValueRO.Position, currentTarget);

            if(dist < 0.1f) { buffer.RemoveAt(0); }
        }
    }
}
