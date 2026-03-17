using System;
using System.Collections.Generic;

Peasant pop = new Peasant();

for(int i=0;i<4;i++){
pop.BuildBarracks();}

public class Bank
{
    private static Bank _instance;

    public static Bank Instance
    {
        get
        { 
        if(_instance == null)
            {
                _instance = new Bank();
            }
            return _instance;
            
        } 
    }
    public int Gold {get; private set;} = 1000;

    public void Spend(int amount)
    {
        if(amount <= Gold)
        {
            Gold -= amount;
            Console.WriteLine($"Bank: {amount} spent. Remaining: {Gold}");
        }
        else { Console.WriteLine($"Only {Gold}, no gold"); }
    }
}

public class Peasant
{
    public void BuildBarracks()
    {
        Bank.Instance.Spend(300);
    }
}
