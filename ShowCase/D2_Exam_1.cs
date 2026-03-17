using System;
using System.Collections.Generic;

List<EnemyUnit> EnemyGroup = new List<EnemyUnit>();
Radar MyRad = new Radar();
LaserGun Laser = new LaserGun();
BeamPool<LaserBeam> LaserBeamPool = new BeamPool<LaserBeam>();

Random UnitType = new Random();
Random XYtype = new Random();

for(int i = 0; i< 25; i++)
{
    if(UnitType.Next(0,2)== 1)
    {
        EnemyGroup.Add(new Asteroid(XYtype.Next(1,100),XYtype.Next(1,100)));
    }
    else
    {
        EnemyGroup.Add(new AlienShip(XYtype.Next(1,100),XYtype.Next(1,100)));
    }
}

bool EndGame = false;
while(EndGame == false){
    for(int i = EnemyGroup.Count -1; i>=0; i--)
    {
    if(EnemyGroup[i].IsAlive == false)
    {
    EnemyGroup.RemoveAt(i); 
    }
    }
    int number = 0;
if(CommandCenter.Center.Score >= 100 ) {EndGame = true;}

Console.WriteLine($"Enemy Group Stat:");
    foreach(var unit in EnemyGroup)
{
    unit.X-=5;
    unit.Y-=5;
    Console.WriteLine($"{unit.Name} , HP = {unit.HP} , Speed = {unit.Speed} X = {unit.X}, Y = {unit.Y}"); 
}

    Console.WriteLine($"Stats: Score: {CommandCenter.Center.Score} Energy: {CommandCenter.Center.Energy} Cap Rechargers count {CommandCenter.Center.CapRecharger}");

    Console.WriteLine($"What do you want to use? 1 - Radar , 2 - Cap Recharger, 3 - Nothing");

int y = 0;
while(y != 1 && y != 2 && y != 3)
    {
        y = Convert.ToInt32(Console.ReadLine());
        if(y == 2)
        {
            CommandCenter.Center.Recharge();
        }
    else if(y == 1){
    Console.WriteLine($"What do you want to see? 1 - Only Alien Ships or 2 - all enemies in 50 km?");

int x = 0;
while(x != 1 && x != 2)
{
x = Convert.ToInt32(Console.ReadLine());
if(x != 1 && x != 2)
    {
        Console.WriteLine("Wrong Number!");
    }
foreach(var unit in EnemyGroup)
    {
        number += 1;
        MyRad.EnemyAnalyze(x,number,unit);
    }
}
    }
    }
    foreach(var unit in EnemyGroup)
    {
        var Shoot = LaserBeamPool.Get();
        Laser.Shoot(unit);
        LaserBeamPool.Return(Shoot);
    }



}
public class CommandCenter
{
    private static CommandCenter? _center;

    public static CommandCenter Center
    {
        get
        {
            if(_center == null)
            {
                _center = new CommandCenter();
            }
            return _center;
        }
    }
    private CommandCenter() {}

    public int Score { get; private set; } = 0;
    public int MaxEnergy { get; private set; } = 1500;


    private int _capRech = 3;
    private int MaxRech = 3;
    public int CapRecharger 
    {
        get
        {
            return _capRech;
        }
        set
        {
            if(value < 0)
            {
                _capRech = 0;
                Console.WriteLine("No Rechargers!");
            }
            else if(value > MaxRech)
            {
                _capRech = 3;
                Console.WriteLine($"Cap Rechargers is full! {_capRech}/{MaxRech}");
            }
            else _capRech = value;
        }
    }

    public int RechargeValue { get; private set; } = 400;


    private int _energy = 1500;
    public int Energy
    {
        get
        {
            return _energy;
        } 
        set
        {
            if(value < 0) 
            {
                _energy = 0;
                Console.WriteLine("No energy!");
            }
            else if(value > MaxEnergy)
            {
               _energy = MaxEnergy;
               Console.WriteLine($"Energy is full! {_energy}/{MaxEnergy}");
            }
            else _energy = value;
        }
    }

    public void Recharge()
    {
        if(_capRech > 0)
        {
        if(_energy < MaxEnergy)
        {
        _energy += RechargeValue;
        _capRech -= 1;
        Console.WriteLine($"Use Cap Recharge! +{RechargeValue}, now value is {_energy}/{MaxEnergy}");
        }
        else Console.WriteLine($"We have a full energy! {_energy}/{MaxEnergy}");
        }
        else Console.WriteLine($"We are not have CapRechargers!");
    }

    public void AddScore(int count)
    {
        Score += count;
    }

    public void Shoot(int count)
    {
        Energy -= count;
    }

}


public class EnemyUnit
{
    public string Name { get; private set; }
    private int _x;
    public int X { get{ return _x; }
        set
        {
            if(value <= 0)
            {
                _x = 0;
            }
            else _x = value;
        } 
        }

    private int _y;
    public int Y { get{return _y;}
        set
        {
             if(value <= 0)
            {
                _y = 0;
            }
            else _y = value;
        } 
        }

    public int Speed { get; private set; }

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
                _hp = 0;
            }
            else _hp = value;
        }
    }

    public int ScoreAdd { get; private set; }

    public EnemyUnit(string name, int x,int y, int speed,int hp,int scoreadd)
    {
        Name = name;
        X = x;
        Y = y;
        Speed = speed;
        HP = hp;
        ScoreAdd = scoreadd;
    }

    public bool IsAlive{ get; private set; } = true;

    public virtual void Damage(int count)
    {
        HP-=count;
        if(HP <= 0)
        {
            IsAlive = false;
            CommandCenter.Center.AddScore(ScoreAdd);
        }

    }
}

public class  Asteroid : EnemyUnit
{
    public Asteroid(int x,int y) : base("Asteroid",x,y,15,10,5){}

    public override void Damage(int count)
    {
        base.Damage(count);
        if(IsAlive == false)
        {
            Console.WriteLine($"Asteroid is Dead + {ScoreAdd}");
        }

    }
}
public class  AlienShip : EnemyUnit
{
    public AlienShip(int x,int y) : base("Alien Ship",x,y,30,20,10){}
        public override void Damage(int count)
    {
        base.Damage(count);
        if(IsAlive == false)
        {
            Console.WriteLine($"Alien Ship is Dead + {ScoreAdd}");
        }

    }
}

public class Radar
{
    public int X {get; private set;} = 0;
    public int Y {get; private set;} = 0;
    public void EnemyAnalyze(int count,int number,EnemyUnit unit)
    {
        if(count == 1)
        {
            if(unit is AlienShip ship)
            {
                Console.WriteLine($"Target ID: {number} , {ship.Name} on radar!");
            }
        }
        else if(count == 2)
        {
            int distance = (int)Math.Sqrt(Math.Pow(unit.X-X,2)+Math.Pow(unit.Y-Y,2));
            if(distance < 50)
            {
                Console.WriteLine($"Enemy unit ID: {number} , Unit: {unit.Name} at {distance} km");
            }
        }
    }
}


public class LaserGun
{
    public int Damage {get; private set;} = 5;
    public int EnergyShoot {get; private set;} = 40;
    public int MaxRange { get; private set; } = 30;
    public int X {get; private set;} = 0;
    public int Y {get; private set;} = 0;

    public void Shoot(EnemyUnit unit)
    {
        int distance = (int)Math.Sqrt(Math.Pow(unit.X-X,2)+Math.Pow(unit.Y-Y,2));
        if(distance < MaxRange)
        {
            Console.WriteLine($"LaserGun is shoot enemy target: {unit.Name} at {distance} km");
            unit.Damage(Damage);
            CommandCenter.Center.Shoot(EnergyShoot);
        }
        else Console.WriteLine($"Too much range to target {unit.Name}");
    }
}

public class LaserBeam{}

public class BeamPool<T> where T : new()
{
    private Queue<T> _pool = new Queue<T>();
    public T Get()
    {
        if(_pool.Count > 0)
        {
            return _pool.Dequeue(); 

        }
        else
        {
            return new T();
        }
    }
    public void Return(T item)
    {
        _pool.Enqueue(item); 
    }
}
