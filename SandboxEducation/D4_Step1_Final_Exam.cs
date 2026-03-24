using System;
using System.Collections.Generic;
using System.Linq;
Point2D point3D = new Point2D(10,5);
Faction factRed = Faction.Red; // И как это присваивать

ArenaManager.ArenaManage.AliveUnits.Add(new Unit("Brok",factRed,point3D));
public struct Point2D
{
    public int X;
    public int Y;

    public Point2D(int x , int y) { X = x; Y = y; }
}

public enum Faction
{
    Red,
    Blue
}

public class ArenaManager
{
    private static ArenaManager? _arenamanager;

    public static ArenaManager ArenaManage
    {
        get
        {
            if(_arenamanager == null)
            {
                _arenamanager = new ArenaManager();
            }
            return _arenamanager;
        }
    }

    public List<Unit> AliveUnits = new List<Unit>();
    private ArenaManager() { }

    public Unit? GetClosestEnemy(Unit request) // Забыл про способ написания методов где нужно возвращать что либо - записать
    {
        List<Unit> Enemies = AliveUnits.Where(u => request._faction != u._faction).ToList();

        Unit? ClosestEnemy = Enemies.OrderByDescending(u =>u.dist = (int)Math.Sqrt(Math.Pow(u.point2D.X - request.point2D.X,2)+Math.Pow(u.point2D.Y - request.point2D.Y,2)))
        .FirstOrDefault();

        if(ClosestEnemy != null)
        {
            return ClosestEnemy;
        }
        else return null;
    }
}

public class Unit
{
    private State? _currentState;

    public void ChangeState(State newState)
    {
        _currentState = newState;
    }

    public void Update()
    {
        _currentState?.Action(this);
    }
    public string Name { get; private set; }

    public int dist = 0;

    private int hp = 100;
    public int Health
    {
        get { return hp; }
        set
        {
            if(value <= 0) {hp = 0; OnDeath?.Invoke(this);}
            else hp = value;
        }
    }

    public Faction? _faction;
    
    public Point2D point2D;

    public Action<Unit>? OnDeath;

    public Unit(string name,Faction fact,Point2D point)
    {
        Name =  name;
        _faction = fact;
        point2D = point;
    }

    public void TakeDamage(int dmg)
    {
        Health -= dmg;
    }


}

public interface State
{
    void Action(Unit unit);
}

public class SearchState : State
{
    public void Action(Unit unit)
    {
        Unit? unit2 = ArenaManager.ArenaManage.GetClosestEnemy(unit); //this не работает
        if(unit2 != null)
        {
            unit.ChangeState(new AttackState(unit2));
        }
    }
}

public class AttackState : State
{
     private readonly Unit _unit;
    public AttackState(Unit unit)
    {
        _unit = unit; // Почему я должен лезть в гугл, мы такого не разбирали и близко и я без понятия как возвращать юнит , readonly впервые вижу
    }
    public void Action(Unit unit)
    {
        unit.TakeDamage(20);
        if(unit.Health <= 0)
        {
            unit.ChangeState(new SearchState());
        }
    }
}

public class Projectile{}

public class ProjectPool<T> where T : new()
{
    private Queue<T> _pool = new Queue<T>();

    public T Get()
    {
      if (_pool.Count > 0) return _pool.Dequeue(); 
      else return new T(); 
    }

    public void Return(T item) 
    {
        _pool.Enqueue(item); 
    }

}