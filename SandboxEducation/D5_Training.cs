

Rookie Jimmi = new Rookie("Jimmi");
Veteran John = new Veteran("John Uik",30);

Console.WriteLine($"Name: {Jimmi.Name}, HP: {Jimmi.HP}, level: {Jimmi.Level} ");
Console.WriteLine($"Name: {John.Name}, HP: {John.HP}, level: {John.Level} ");
public class Mercenary
{
    public string Name { get; } // как я понял только для чтения = убрать сет
    public int HP { get; }
    public int Level { get; }

    public Mercenary(string name,int hp, int level){ Name = name; HP = hp; Level = level; }
}

public class Rookie : Mercenary
{
    public Rookie(string rookieName) : base(rookieName,100,1) {}
}

public class Veteran : Mercenary
{
    public Veteran(string vetName,int vetLevel) : base(vetName,vetLevel * 50,vetLevel) {}
}