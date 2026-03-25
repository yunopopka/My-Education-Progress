
Bomb myBomb = new Bomb();
ScoreManager ui = new ScoreManager();

myBomb.OnExplode += ui.AddPoints;

myBomb.TakeDamage(10);


public class Bomb
{
    private int _hp = 10;

    public event Action OnExplode;

    public void TakeDamage(int dmg)
    {
        _hp-=dmg;
        if(_hp <= 0) { Console.WriteLine($"Boom!"); OnExplode?.Invoke(); }
    }
}

public class ScoreManager
{
    int _score = 0;

    public void AddPoints()
    {
        _score += 100;
        Console.WriteLine($"Points added! Score: {_score}");
    }
}