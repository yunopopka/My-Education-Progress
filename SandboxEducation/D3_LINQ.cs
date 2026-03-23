using System.Linq;
using System.Collections.Generic;

List<Target> targets = new List<Target>();

targets.Add(new Target("X-101",75,50));
targets.Add(new Target("Shahed 136",25,25));
targets.Add(new Target("X-101",100,50));
targets.Add(new Target("Iscander",100,100));
targets.Add(new Target("X-59",30,35));

List<Target> Close = targets.Where(t => t.Distance < 50)
                            .OrderByDescending(t => t.Distance)
                            .ToList();
foreach(var unit in Close)
{
    Console.WriteLine($"Name: {unit.Name} ,distance: Threat: {unit.Threat}");
}

Target target = targets.OrderBy(t => t.Distance).FirstOrDefault();

Console.WriteLine($"Closest target is: {target.Name}  distance: {target.Distance}");

List<int> TargDist = targets.Where(t => t.Distance > 40)
                               .Select(t => t.Distance)
                               .ToList();

foreach(var t in TargDist)
{
    Console.WriteLine($"{t}");
}

public class Target
{
    public string Name {get; private set;}
    public int Distance {get; private set;}
    public int Threat {get; private set;}

    public Target(string name, int dist,int threat)
    {
        Name = name;
        Distance = dist;
        Threat = threat;
    }
}