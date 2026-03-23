Drone newDr = new Drone();
ScoreBoard score = new ScoreBoard();

newDr.OnDestroyed += score.AddPoints;

newDr.TakeDamage(50);

public class Drone
{
    public event Action<int>? OnDestroyed;
    private int _hp = 50;
    public int hp { 
        get{ return _hp; }
        set
        {
            if(value <= 0)
            {
                _hp = 0;
                OnDestroyed?.Invoke(100);
                Console.WriteLine($"Drone is destroyed!");
            }
            else _hp = value;
        }
        }

        public void TakeDamage(int damage)
    {
        if(_hp != 0)
        hp -= damage;
    }
}

public class ScoreBoard
{
    private int score = 0;
    public int TotalScore
    {
        get{return score;}
        set
        {
            if(value > 0)
            score = value;
        }
    }

    public void AddPoints(int points)
    {
       TotalScore += points;
       Console.WriteLine($"Tablo: nachisleno: {points} points,all: {score}");
    }
}