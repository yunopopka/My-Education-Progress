using System;
using System.Collections.Generic;

List<Unit> newList = new List<Unit>();
newList.Add(new Unit("Popa",40,50,"Sniper"));
newList.Add(new Unit("Pone",50,50,"Medic"));
newList.Add(new Unit("Ripa",20,80,"Tank"));
newList.Add(new Unit("Boo",25,30,"Recon"));
newList.Add(new Unit("Brok-20",20,20,"Drone"));

SelectionBox Boxi = new SelectionBox();

Boxi.Select(newList,X => X.HP < X.MaxHP);
Boxi.Select(newList,Y=> Y.Role == "Medic");
public class Unit
{
    public string Name {get; private set;}
    public int HP {get; private set;}
    public int MaxHP {get; private set;}
    public string Role {get; private set;}

    public Unit(string name, int hp, int maxhp,string role)
    {
        Name = name;
        HP = hp;
        MaxHP = maxhp;
        Role = role;
    }
}

public class SelectionBox
{
    public void Select(List<Unit> army, Func<Unit,bool> condition)
    {
        foreach(var unit in army)
        {
            if(condition(unit) == true)
            {
                Console.WriteLine($"Videlen {unit.Name} ({unit.Role}) - HP: {unit.HP}/{unit.MaxHP}");
            }
        }
    }
}