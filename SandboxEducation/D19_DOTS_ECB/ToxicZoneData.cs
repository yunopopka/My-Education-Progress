using Unity.Entities;
using Unity.Mathematics;

// 1. Синглтон токсичной зоны (Почтовый ящик)
public struct ToxicZoneSingleton : IComponentData
{
    public float3 Center;
    public float Radius;
    public float DamagePerSecond;
}

// 2. Здоровье (висит на каждом юните)
public struct _Health : IComponentData
{
    public float Current;
}