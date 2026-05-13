using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct TankVisual : IComponentData
{
    public float CurrentPitch; // Текущий наклон корпуса (в радианах)
    public float WheelRadius;  // Радиус катка (для расчета вращения)
    public float WheelAngle;   // На какой угол сейчас повернуты катки
}

public readonly partial struct TankVisualAspect : IAspect
{
    private readonly RefRW<LocalTransform> transform; // Это трансформ КОРПУСА
    private readonly RefRW<TankVisual> visual;


    public void UpdateVisuals(float dt, in TankTelemetry telemetry)
    {
        float targetPintch = telemetry.CurrentAcceleration * -0.05f;
        visual.ValueRW.CurrentPitch = math.lerp(visual.ValueRO.CurrentPitch, targetPintch, 5f * dt);


        transform.ValueRW.Rotation = quaternion.RotateX(visual.ValueRO.CurrentPitch);

        visual.ValueRW.WheelAngle += (telemetry.CurrentSpeed * dt) / visual.ValueRO.WheelRadius;
    }
}