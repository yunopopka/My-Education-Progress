using System.Linq;
using System.Collections.Generic;

List<Target> targets = new List<Target>();
Sensor sensor = new Sensor();
targets.Add(new Target("Dodo",20));
targets.Add(new Target("Doda",50));
targets.Add(new Target("Dods",10));
targets.Add(new Target("Dodi",30));
targets.Add(new Target("Dodb",40));

sensor.OnPerimeterBreached += AlarmSystem.Instance.SoundAlarm;

sensor.Scan(targets);
public class AlarmSystem
{
    private static AlarmSystem? _instance;

    public static AlarmSystem Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new AlarmSystem();
            }
            return _instance;
        }
    }
    private AlarmSystem() {}

    public void SoundAlarm(int threatCount)
    {
        Console.WriteLine($"Alarm! Detected {threatCount} intruders!");
    }
}

public class Sensor
{
    public event Action<int>? OnPerimeterBreached;
    
    public void Scan(List<Target> allTargets)
    {
        List<Target> PerimeterTargets = allTargets.Where(t=>t.Distance <20).ToList();

        if(PerimeterTargets.Count > 0)
        {
            OnPerimeterBreached.Invoke(PerimeterTargets.Count);
        }
    }
}

 public class Target

{

    public string Name {get; private set;}

    public int Distance {get; private set;}

    public Target(string name, int dist)

    {

        Name = name;

        Distance = dist;

    }

} 