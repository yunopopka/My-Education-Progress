

Drone drone = new Drone();

drone.SetState(new IdleState());
drone.Work();
drone.SetState(new ChaseState());
drone.Work();

Bot bot = new Bot("Brobe");
bot.SetState(new CombatState());
bot.Update();
bot.Update();
bot.Update();
public class Bot
{
    public string Name {get; private set;}

    private int _hp = 100;
    public int HP {
        get
    { return _hp;
    }
        set
        {
            if(value <= 0){ _hp = 0; }
            else _hp = value;
        }
    } 

    public Bot(string name){ Name = name; }

    private IState_2? _currentState;

    public void SetState(IState_2 newState)
    {
        _currentState = newState;
    }

    public void Update()
    {
        _currentState.Execute(this);
    }
}

public class CombatState : IState_2
{
    public void Execute(Bot context)
    {
        context.HP -= 40;
        Console.WriteLine($"Bot is  fighting!");
        if(context.HP < 30)
        {
            context.SetState(new RetreatState());
        }
    }
}

public class RetreatState : IState_2
{
    public void Execute(Bot context)
    {
        Console.WriteLine($"Critical hit! coming bck to base!");
    }
}

public interface IState_2
{
    void Execute(Bot context);
}

public interface IState
{
    void Execute();
}

public class IdleState : IState
{
    public void Execute()
    {
        Console.WriteLine($"Drone: Scaning");
    }
}

public class ChaseState : IState
{
    public void Execute()
    {
        Console.WriteLine($"Drone: Flying to the target");
    }
}

public class Drone
{
    private IState? _currentState;

    public void SetState(IState newState)
    {
        _currentState = newState;
    }

    public void Work()
    {
        if(_currentState != null)
        {
            _currentState.Execute();
        }
    }
}