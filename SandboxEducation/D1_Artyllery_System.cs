using System;
using System.Collections.Generic;

Headquarters newHead = new Headquarters();
Artillery Arty = new Artillery(0,0,50,60);
List<Unit> UnitList = new List<Unit>();
UnitList.Add(new ScoutCar("Scout",10,10));
UnitList.Add(new HeavyTank("Heavy Tank",30,40)); 
UnitList.Add(new ScoutCar("Scout 2",100,100));

foreach(var unit in UnitList)
{
    unit.OnDeath += newHead.ConfirmKill;
}

foreach(var unit in UnitList)
{
    Arty.FireAt(unit);
}

public class Unit
{
    public string Name { get; private set; }
    public int X { get; private set; }
    public int Y { get; private set; }

    public int Armor { get; private set; }

    public event Action<string>? OnDeath; 

    private int _hp;

    public int HP
    {
        get 
        { 
            return _hp; 
        }
        set
        {
         if(value < 0)
            {
            OnDeath?.Invoke(Name);
            _hp = 0;
            }
            else
            {
                _hp = value;
            }
        }
    }

    public Unit(string name,int x, int y, int armor,int hp)
    {
        Name = name;
        X = x;
        Y = y;
        Armor = armor;
        HP = hp;
    }

    public virtual void TakeDamage(int incomingDamage)
    {
        int ClearDamage = incomingDamage - Armor;

        if(ClearDamage <= 1)
        {
            HP -= 1;
        }
        else
        {
            HP -= ClearDamage;
        }
    }
}

public class HeavyTank : Unit
{
    public HeavyTank(string name, int x, int y, int armor = 15,int hp = 200) : base(name,x,y,armor,hp) {} 
}

public class ScoutCar : Unit
{
    public ScoutCar(string name, int x, int y, int armor = 2 ,int hp = 50) : base(name,x,y,armor,hp) {}
}

public class Artillery
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public int MaxRange { get; private set; }
    public int Damage { get; private set; }

    public Artillery(int x, int y, int maxrange,int damage)
    {
        X = x;
        Y = y;
        MaxRange = maxrange;
        Damage = damage;
    }

    public void FireAt(Unit target)
    {
        int distance = (int)Math.Sqrt( Math.Pow(target.X-X,2) + Math.Pow(target.Y-Y,2) );

        if(distance <= MaxRange)
        {
            target.TakeDamage(Damage);
            Console.WriteLine($"Hit a {target.Name} from distance: {distance}");
        }
        else if(distance >= MaxRange)
        {
            Console.WriteLine($"Overshoot! {target.Name} is out of range (Range: {distance}).");
        }
    }
}

public class Headquarters
{
    public void ConfirmKill (string unitName)
    {
        Console.WriteLine($"HEADQUARTERS: Confirming the destruction of enemy unit - {unitName}");
    }
}