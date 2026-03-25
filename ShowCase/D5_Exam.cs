// Ядро для симулятора космической добычи. 
// Флот автономных дронов-шахтеров вылетает с Материнского Корабля, ищет астероиды, добывает из них ресурсы и пополняет трюм базы.

using System;
using System.Linq;
using System.Collections.Generic;

Random Rand = new Random();
List<Asteroid> newAsteroids = new List<Asteroid>();

MiningDrone drone1 = new MiningDrone("Drone 1",new Vector2D(0,0),OreType.Gold);
MiningDrone drone2 = new MiningDrone("Drone 2",new Vector2D(5,5),OreType.Iron);
MiningDrone drone3 = new MiningDrone("Drone 3",new Vector2D(10,10),OreType.Uranium);

for(int i = 0 ; i < 5 ; i++)
{
    newAsteroids.Add(new Asteroid(new Vector2D(Rand.Next(0,40),Rand.Next(0,40)),OreType.Gold));
    newAsteroids.Add(new Asteroid(new Vector2D(Rand.Next(0,40),Rand.Next(0,40)),OreType.Iron));
    newAsteroids.Add(new Asteroid(new Vector2D(Rand.Next(0,40),Rand.Next(0,40)),OreType.Uranium));
}
for(int i = 0 ; i < 15 ; i++)
{
    MotherShip.MamaShip.NewAsteroid(newAsteroids[i]);
}

for(int i = 0 ; i < 8 ; i++)
{
    Console.WriteLine($"Round:{i}");

        drone1.Update(); 
        drone2.Update(); 
        drone3.Update();
}

MotherShip.MamaShip.TotalOre();

public struct Vector2D { public int X; public int Y; public Vector2D(int x,int y) { X = x; Y = y; } }

public enum OreType { Iron,Gold,Uranium }

public class MotherShip
{
    private static MotherShip? _mothership;
    public static MotherShip MamaShip { get { if(_mothership == null) _mothership = new MotherShip(); return _mothership; } }
    private MotherShip()
    {
        foreach (OreType type in Enum.GetValues(typeof(OreType))) { OreStorage[type] = 0; } // Решил эту проблему с ИИ, я понимал что есть подвох с инициализацией начальной, но как его сделать не знал
    }

    public void NewAsteroid(Asteroid ast){ ActiveAsteroids.Add(ast); ast.OnDepleted += DeleteAsteroid; }

    public void DeleteAsteroid(Asteroid ast)
    {
        AddOre(ast.TypeOre);
        ActiveAsteroids.Remove(ast);
    }

    public void TotalOre()
    {
        Console.WriteLine($"Total Iron: {OreStorage[OreType.Iron]}");
        Console.WriteLine($"Total Gold: {OreStorage[OreType.Gold]}");
        Console.WriteLine($"Total Uranium: {OreStorage[OreType.Uranium]}");
    }

    public Dictionary<OreType,int> OreStorage = new Dictionary<OreType, int>();
    

    private void AddOre(OreType OreType)
    {
        if (OreStorage.ContainsKey(OreType)) OreStorage[OreType] += 50;
    }

    public List<Asteroid> ActiveAsteroids = new List<Asteroid>();

    public Asteroid? GetClosestAsteroid(Vector2D searcherPos, OreType targetOre)
    {
        return ActiveAsteroids.Where(a=> targetOre == a.TypeOre)
                              .OrderBy(a=> GetDistance(searcherPos,a.AstVector))
                              .FirstOrDefault();
    }

    private int GetDistance(Vector2D p1, Vector2D p2)
    {
        return (int)Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
    }

}

public class Asteroid
{
    private bool _isDead = false;
    private int _hp = 100;
    public int Health {
        get{ return _hp; }
        set
        {
            if(value <= 0)
            {
                _hp = 0;
                OnDepleted?.Invoke(this);
                _isDead = true;
                Console.WriteLine($"The {TypeOre} asteroid is depleted");
            }
            else _hp = value;
        }
        } 
    public Vector2D AstVector;
    public OreType TypeOre;

    public Asteroid(Vector2D vector,OreType type) { AstVector = vector; TypeOre = type; }

    public Action<Asteroid>? OnDepleted;

    public void TakeDamage(int dmg){ Health -= dmg; }
}

public class MiningDrone
{
    private IState? _currentState;

    public void SetState(IState state){ _currentState = state; }
    public void Update(){if(_currentState != null){ _currentState?.Action(this); } }
    public string Name {get; private set;}
    public Vector2D DroneVector;
    public OreType TypeOre;

    public MiningDrone(string name,Vector2D vect,OreType oreType)
    {
        Name = name;
        DroneVector = vect;
        TypeOre = oreType;
        SetState(new SearchState());
    }
}

public interface IState{ void Action(MiningDrone drone){ } }

public class SearchState : IState
{
    public void Action(MiningDrone drone)
    {
        Asteroid? aster = MotherShip.MamaShip.GetClosestAsteroid(drone.DroneVector,drone.TypeOre);
        if(aster != null){ drone.SetState(new MineState(aster)); }
        else Console.WriteLine("No asteroids, coming back");
    }
}

public class MineState : IState
{
    private Asteroid _target;
    public MineState(Asteroid asteroid){ _target = asteroid; }
    public void Action(MiningDrone drone)
    {
        _target.TakeDamage(20);
        if(_target.Health <= 0){ drone.SetState(new SearchState()); }
    }
}