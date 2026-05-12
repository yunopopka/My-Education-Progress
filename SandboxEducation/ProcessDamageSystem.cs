using Unity.Burst;
using Unity.Entities;

// Элемент массива
public struct DamageBufferElement : IBufferElementData
{
    public float DamageValue;
}

[BurstCompile]
public partial struct ProcessDamageSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach(var (health,buffer) in SystemAPI.Query<RefRW<Health>,DynamicBuffer<DamageBufferElement>>())
        {
            if(buffer.Length == 0) { continue; }

            float totalDamage = 0;

            for(int i=0; i < buffer.Length; i++)
            {
                totalDamage += buffer[i].DamageValue;
            }

            health.ValueRW.Current -= totalDamage;

            buffer.Clear();
        }
    }
}