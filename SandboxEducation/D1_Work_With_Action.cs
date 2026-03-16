using System;

Radar myRad = new Radar();
MissileTurret MisTur = new MissileTurret();
Siren mySir = new Siren();

myRad.OnEnemyDetected += MisTur.Aim;
myRad.OnEnemyDetected += mySir.PlayAlarm;

myRad.Scan(450);

public class Radar
{
    public Action<int> OnEnemyDetected;

    public void Scan(int distance)
    {
        Console.WriteLine("RADAR: Movement detected");

        OnEnemyDetected?.Invoke(distance);
    }
}

public class MissileTurret
{
    public void Aim(int targetDistance)
    {
        Console.WriteLine($"Turret: Deploying guns. Target at {targetDistance} km.");
    }
}

public class Siren
{
    public void PlayAlarm(int dist)
    {
        Console.WriteLine($"Syren! Alarm! Enemy at {dist} km.");
    }
}