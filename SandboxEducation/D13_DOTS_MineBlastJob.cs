using Unity.Entities;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Collections;

public struct MineTag : IComponentData { }
public struct TankTag : IComponentData { }

public struct MineBlastJob : ITriggerEventsJob
{
    [ReadOnly] public ComponentLookup<MineTag> MineLookup;
    [ReadOnly] public ComponentLookup<TankTag> TankLookup;

    public ComponentLookup<PhysicsVelocity> VelocityLookup;

    public void Execute(TriggerEvent triggerEvent)
    {
        Entity entityA = triggerEvent.EntityA;
        Entity entityB = triggerEvent.EntityB;

        bool isAMine = MineLookup.HasComponent(entityA);
        bool isBaTarget = TankLookup.HasComponent(entityB);

        if(isAMine && isBaTarget)
        {
            PhysicsVelocity speed = VelocityLookup[entityB];
            speed.Linear += math.up() * 50f;

            VelocityLookup[entityB] = speed;

            return;
        }

        

        bool isBMine = MineLookup.HasComponent(entityB);
        bool isATarget = TankLookup.HasComponent(entityA);

        if(isBMine && isATarget)
        {
            PhysicsVelocity speed = VelocityLookup[entityA];
            speed.Linear += math.up() * 50f;

            VelocityLookup[entityA] = speed;

            return;
        }

    }
}