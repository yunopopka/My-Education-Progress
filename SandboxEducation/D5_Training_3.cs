
List<Aircraft> aircrafts = new List<Aircraft>();

aircrafts.Add(new Aircraft("Boeing",false,10));
aircrafts.Add(new Aircraft("Fighter",true,50));
aircrafts.Add(new Aircraft("Small plane",false,5));
aircrafts.Add(new Aircraft("Drone Camikadze",true,20));
aircrafts.Add(new Aircraft("Stealth",true,100));

Aircraft? aircraft = aircrafts.Where(u=>u.IsEnemy == true)
                              .OrderBy(u => u.Distance)
                              .FirstOrDefault();

if(aircraft != null)
{
    Console.WriteLine($"Closest enemy: {aircraft.Name}");
}
else Console.WriteLine($"No enemies");
public class Aircraft
{
    public string Name {get; private set;}
    public bool IsEnemy {get; private set;}
    public int Distance {get; private set;}

    public Aircraft(string name,bool enemy,int dist) { Name = name; IsEnemy = enemy; Distance = dist; }
}