using Unity.Entities;

public struct DroneSpawner : IComponentData
{
    public Entity DronePrefab;  // Чертеж дрона
    public float Timer;         // Текущий таймер (будем отнимать время)
    public float SpawnInterval; // Интервал спавна (например, 3 секунды)
}

public struct DroneLifetime : IComponentData
{
    public float TimeLeft; // Сколько секунд осталось жить (например, 5)
}