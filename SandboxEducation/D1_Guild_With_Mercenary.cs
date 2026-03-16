using System.Collections.Generic;

Guild myGuild = new Guild();
myGuild.AddMember("Ghost",new Sniper("Ghost"));
myGuild.AddMember("Neo",new Hacker("Neo"));

foreach(var unit in myGuild.Roster.Values)
{
    unit.Report();
    if(unit is Sniper unitSniper)
    {
        unitSniper.Aim();
    }
    else if(unit is Hacker unitHacker)
    {
        unitHacker.Hack();
    }

}


public class Mercenary
{
    public string Name { get; private set; }

    public virtual void Report()
    {
        Console.WriteLine($"Mercenary {Name} on communication");
    }

    public Mercenary(string name) { Name = name; }
}

public class Sniper : Mercenary
{
    public Sniper(string Name) : base(Name){}

    public override void Report()
    {
        Console.WriteLine($"Sniper {Name} on communication");
    }

    public void Aim() { Console.WriteLine($"Sniper see a target"); }
}

public class Hacker : Mercenary
{
    public Hacker(string Name) : base(Name){}

    public override void Report()
    {
        Console.WriteLine($"Haker {Name} on communication");
    }

    public void Hack() { Console.WriteLine($"Hacker is hacking terminal"); }
}

public class Guild
{
    public Dictionary<string, Mercenary> Roster {get; private set;} = new Dictionary<string, Mercenary>();

    public void AddMember(string callsign, Mercenary m)
    {
        Roster.Add(callsign,m);
    }
}