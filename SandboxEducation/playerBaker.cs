using Unity.Entities;
using UnityEngine;

public struct Health : IComponentData
{
    public float Max;
    public float Current;
}

public struct PlayerTag : IComponentData { }


public class PlayerAuthoring : MonoBehaviour
{
    public float StartHealth;
    public bool IsPlayer;
}

public class PlayerBaker : Baker<PlayerAuthoring>
{
    public override void Bake(PlayerAuthoring authoring)
    {
        Entity enty = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(enty, new Health
        {
            Max = authoring.StartHealth,
            Current = authoring.StartHealth
        });
        if (authoring.IsPlayer)
        {
            AddComponent(enty, new PlayerTag { });
        }
    }
}