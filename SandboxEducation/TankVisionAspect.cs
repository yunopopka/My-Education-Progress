using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.VisualScripting; // Подключаем физику

public struct TankSensors : IComponentData
{
    public float RayDistance; // Как далеко смотрим (например, 10 метров)
    public float AvoidanceForce; // Насколько сильно "отшатываемся" от стены
}

public readonly partial struct TankVisionAspect : IAspect
{
    private readonly RefRO<LocalTransform> transform;
    private readonly RefRO<MoveCommand> command;
    private readonly RefRO<TankSensors> sensors;

    // Метод возвращает БЕЗОПАСНЫЙ вектор направления
    public float3 GetSafeDirection(ref PhysicsWorld physicsWorld)
    {
        float3 offsetToTarget = command.ValueRO.TargetPosition - transform.ValueRO.Position;
        offsetToTarget.y = 0; 

        float3 desiredDirection = math.normalize(offsetToTarget);

        RaycastInput rayInput = new RaycastInput
        {
            Start = transform.ValueRO.Position,
            End = transform.ValueRO.Position + new float3(0,1.0f,0) + (transform.ValueRO.Forward() * sensors.ValueRO.RayDistance),
            Filter = CollisionFilter.Default  
        };

        if(!physicsWorld.CastRay(rayInput,out RaycastHit hit)) { return desiredDirection; }
        else
        {
            float3 avoidanceVector = hit.SurfaceNormal * sensors.ValueRO.AvoidanceForce;
            float3 currentDirection = desiredDirection + avoidanceVector;

            currentDirection.y = 0;

            return math.normalize(currentDirection);
        }
    }
}
