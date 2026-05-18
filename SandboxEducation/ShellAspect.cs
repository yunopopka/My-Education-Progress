using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct ArtilleryShell : IComponentData
{
    public float3 Velocity; // Куда и как быстро летит (вектор направления * скорость)
    public float Gravity;   // Сила притяжения (например, 9.8f)
    public float LifeTime;  // Через сколько секунд исчезнет сам по себе
}

public readonly partial struct ShellAspect : IAspect
{
    public readonly Entity Self; // ID самого снаряда для удаления
    private readonly RefRW<LocalTransform> transform;
    private readonly RefRW<ArtilleryShell> shell;

    public void Fly(float dt, EntityCommandBuffer.ParallelWriter ecb, int sortKey)
    {
        shell.ValueRW.LifeTime -= dt;

        transform.ValueRW.Position += shell.ValueRO.Velocity * dt;

        shell.ValueRW.Velocity.y -= shell.ValueRO.Gravity * dt;

        transform.ValueRW.Rotation = quaternion.LookRotationSafe(shell.ValueRO.Velocity, math.up());

        if(shell.ValueRO.LifeTime <= 0 || transform.ValueRO.Position.y < 0) { ecb.DestroyEntity(sortKey,Self); }
    }
}
