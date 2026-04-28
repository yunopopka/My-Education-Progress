using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smelter : MonoBehaviour
{
    private Queue<string> oreQueue = new Queue<string>();
    private Dictionary<string, int> recipes = new Dictionary<string, int>();
    private string correntOre;
    private void Awake()
    {
        recipes.Add("IronOre", 5);
        recipes.Add("GoldOre", 20);
        recipes.Add("Scrap", 2);
    }
    private void Start()
    {
        StartCoroutine(ProcessQueue());
    }
    public void AddOre(string oreName)
    {
        oreQueue.Enqueue(oreName);
        Debug.Log("Added: " + oreName);
    }

    private IEnumerator ProcessQueue()
    {
        while (true)
        {
            // Проверяем, есть ли что-то в очереди
            if (oreQueue.Count > 0)
            {
                // 1. Берем руду (достаем из очереди)
                correntOre = oreQueue.Dequeue();

                // 2. Печь загудела. Ждем 2 секунды на переплавку
                yield return new WaitForSeconds(2f);

                // 3. Выдаем результат
                if (recipes.ContainsKey(correntOre))
                {
                    // Берем из словаря значение по ключу и выводим в лог!
                    Debug.Log("Переплавлено! Получено слитков: " + recipes[correntOre]);
                }
            }
            else
            {
                // Если очередь пуста, просто ждем 1 кадр и проверяем снова
                // (Без этого yield return null пустой цикл while(true) намертво повесит Unity)
                yield return null;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) { AddOre("IronOre"); }
        if (Input.GetKeyDown(KeyCode.Y)) { AddOre("GoldOre"); }
        if (Input.GetKeyDown(KeyCode.U)) { AddOre("Scrap"); }
    }
}
