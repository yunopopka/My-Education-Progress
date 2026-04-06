using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

[BurstCompile]public partial struct ShockwaveSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float time = SystemAPI.Time.DeltaTime;

        //     public float BaseWave; Базовый размер взрыва,  public float GrowSpeed; Скорость расширения
        // Первая попытка которая потом была исправлена

    
        foreach(var(wave,speed)in SystemAPI.Query<RefRW<Shockwave>, RefRO<Shockwave>>())
        {
            float ExtensionSpeed = math.mul(speed.ValueRO.GrowSpeed, time);

            wave.ValueRW.BaseWave = wave.ValueRO.BaseWave + ExtensionSpeed;
        }
    }
}
