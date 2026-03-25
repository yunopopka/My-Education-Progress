// Это уже исправленный код с помощью ИИ, что бы я сюда мог смотреть как за примером, по большей части я написал его нормально но снова споткнулся :(


using System;
using System.Linq;
using System.Collections.Generic;

// 1. НАСТРОЙКА СИМУЛЯЦИИ
Random RandCor = new Random();
Camp.Camping.AddItem(Loot.Medicine, 1); // Даем 1 аптечку
ProjectPool<Bullet> bulletPool = new ProjectPool<Bullet>(); // Пул пуль

// Создаем Выживших
Survivor s1 = new Survivor(new Point(10, 10));
Survivor s2 = new Survivor(new Point(12, 12));
s2.TakeDamage(70); // Раним второго, чтобы у него было 30 HP

// Создаем Зомби (один близко, другой далеко)
Zombie z1 = new Zombie(new Point(15, 15)); // Близко к выжившим (дистанция 5-7)
Zombie z2 = new Zombie(new Point(50, 50)); // Далеко (никто его не тронет)

// Регистрируем в лагере (и сразу подписываем лагерь на их смерть!)
Camp.Camping.RegisterUnit(s1);
Camp.Camping.RegisterUnit(s2);
Camp.Camping.RegisterUnit(z1);
Camp.Camping.RegisterUnit(z2);

Console.WriteLine("--- СИМУЛЯЦИЯ ЗАПУЩЕНА ---");

// 2. ИГРОВОЙ ЦИКЛ
for (int i = 0; i < 5; i++)
{
    Console.WriteLine($"\n[ Ход {i + 1} ]");
    // Используем ToList() для защиты от ошибки при удалении мертвых!
    foreach (var unit in Camp.Camping.AlliveUnits.ToList())
    {
        unit.Update();
    }
}


// ================== АРХИТЕКТУРА ==================

public struct Point { public int X, Y; public Point(int x, int y) { X = x; Y = y; } }
public enum Loot { Food, Medicine, Ammo }
public class Bullet { } // Пустой класс пули

public class ProjectPool<T> where T : new()
{
    private Queue<T> _pool = new Queue<T>();
    public T Get() { return _pool.Count > 0 ? _pool.Dequeue() : new T(); }
    public void Return(T item) { _pool.Enqueue(item); }
}

public class Camp
{
    private static Camp? _camping;
    public static Camp Camping { get { if (_camping == null) _camping = new Camp(); return _camping; } }
    private Camp() { }

    public List<Unit> AlliveUnits = new List<Unit>();
    public Dictionary<Loot, int> Warehouse = new Dictionary<Loot, int>();
    public ProjectPool<Bullet> AmmoPool = new ProjectPool<Bullet>(); // Лагерь хранит пул пуль

    private Random GiveLoot = new Random();

    public void AddItem(Loot ItemType, int count)
    {
        if (Warehouse.ContainsKey(ItemType)) Warehouse[ItemType] += count;
        else Warehouse[ItemType] = count;
    }

    // Метод для добавления юнитов (сразу подписывает на событие смерти)
    public void RegisterUnit(Unit u)
    {
        AlliveUnits.Add(u);
        u.Death += DeleteUnit; // ПОДПИСКА!
    }

    public void DeleteUnit(Unit unit)
    {
        AlliveUnits.Remove(unit);
        
        if (unit is Zombie) // Если умер зомби - даем лут
        {
            Loot randomLoot = (Loot)GiveLoot.Next(0, 3); // от 0 до 2 (Food, Medicine, Ammo)
            AddItem(randomLoot, 10);
            Console.WriteLine($"[ЛУТ] С зомби выпало 10 {randomLoot}!");
        }
    }

    public Unit? GetCloseTarget(Unit searcher)
    {
        return AlliveUnits
            .Where(u => u is Zombie) // Ищем только зомби
            .Where(u => GetDistance(searcher.UnitPoint, u.UnitPoint) <= 15) // Ближе 15 метров
            .OrderBy(u => GetDistance(searcher.UnitPoint, u.UnitPoint)) // Сортируем по ближайшим
            .FirstOrDefault();
    }

    // Удобный метод расчета дистанции
    private int GetDistance(Point p1, Point p2)
    {
        return (int)Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
    }
}

public class Unit
{
    public string Name;
    public Point UnitPoint;
    public Action<Unit>? Death;
    private State? _currentState;

    private int _hp = 100;
    public int Health
    {
        get { return _hp; }
        set
        {
            if (value <= 0)
            {
                _hp = 0;
                Console.WriteLine($"[СМЕРТЬ] {Name} уничтожен!");
                Death?.Invoke(this); // КРИЧИМ В ЭФИР!
            }
            else if (value >= 100) _hp = 100;
            else _hp = value;
        }
    }

    public Unit(string name, int hp, Point point)
    {
        Name = name;
        Health = hp;
        UnitPoint = point;
    }

    public void ChangeState(State newState) { _currentState = newState; }
    
    public void Update() { _currentState?.Action(this); }
    
    public void TakeDamage(int damage) { Health -= damage; }
}

public class Zombie : Unit
{
    public Zombie(Point point) : base("Zombie", 100, point) {} // Передаем point в базу!
}

public class Survivor : Unit
{
    public Survivor(Point point) : base("Survivor", 100, point) 
    {
        ChangeState(new GuardState()); // Сразу ставим в режим охраны при создании!
    }
}

// ================== ИИ ВЫЖИВШИХ ==================

public interface State { void Action(Unit unit); }

public class GuardState : State
{
    public void Action(Unit unit)
    {
        Unit? CloseZomb = Camp.Camping.GetCloseTarget(unit);
        
        if (CloseZomb != null)
        {
            Console.WriteLine($"{unit.Name}: Вижу зомби! В бой!");
            unit.ChangeState(new CombatState(CloseZomb));
        }
        else if (unit.Health < 50)
        {
            Console.WriteLine($"{unit.Name}: Я ранен, иду лечиться...");
            unit.ChangeState(new HealState());
        }
        else 
        {
            Console.WriteLine($"{unit.Name}: Периметр чист, здоровье в норме.");
        }
    }
}

public class CombatState : State
{
    private Unit _target;
    public CombatState(Unit enemyToAttack) { _target = enemyToAttack; }

    public void Action(Unit unit)
    {
        if (_target.Health > 0)
        {
            // Берем пулю из пула
            Bullet b = Camp.Camping.AmmoPool.Get();
            Console.WriteLine($"{unit.Name}: Стреляю в зомби!");
            _target.TakeDamage(60); // Наносим сильный урон
            Camp.Camping.AmmoPool.Return(b); // Возвращаем пулю
        }
        else
        {
            // Если зомби умер - возвращаемся в охрану
            unit.ChangeState(new GuardState());
        }
    }
}

public class HealState : State
{
    public void Action(Unit unit)
    {
        if (Camp.Camping.Warehouse.ContainsKey(Loot.Medicine) && Camp.Camping.Warehouse[Loot.Medicine] > 0)
        {
            Camp.Camping.Warehouse[Loot.Medicine]--; // Списываем аптечку!
            unit.Health = 100;
            Console.WriteLine($"{unit.Name}: Подлечился! Возвращаюсь на пост.");
            unit.ChangeState(new GuardState());
        }
        else
        {
            Console.WriteLine($"{unit.Name}: Медикаментов нет, терплю боль...");
            unit.ChangeState(new GuardState());
        }
    }
}