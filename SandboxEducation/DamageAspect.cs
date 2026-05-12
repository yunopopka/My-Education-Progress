using Unity.Entities;

public readonly partial struct DamageAspect : IAspect
{
    public readonly Entity self;

    private readonly RefRW<Health> health;
    private readonly DynamicBuffer<DamageBufferElement> buffer;

    public void TakeDamageFromBuffer()
    {
        if (buffer.Length == 0) { return; } // континуе не прошел, тыкнул брейк

        float totalDamage = 0;

        for (int i = 0; i < buffer.Length; i++)
        {
            totalDamage += buffer[i].DamageValue;
        }

        health.ValueRW.Current -= totalDamage;

        buffer.Clear();
    }
}