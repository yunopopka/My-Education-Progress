using System;
using UnityEngine;

public class EngingeBuild : MonoBehaviour
{
    public BuildingPool pool;
    public _EconomyManager manager;

    private int ChangeBuilding = 1;
    public static event Action<int> BuildCost;


    public void ButtonWall() { ChangeBuilding = 1; }
    public void ButtonTurret() { ChangeBuilding = 2; }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
           
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            
            if (Physics.Raycast(ray, out hit, 100f))
            {
                
                if (hit.collider.CompareTag("Ground"))
                {
                    if (manager.money >= 50)
                    {
                        if(ChangeBuilding == 1)
                        {
                            GameObject obj = pool.GetPooledWall();
                            obj.transform.position = hit.point;
                            BuildCost?.Invoke(20);
                            obj.SetActive(true);
                        }
                        else
                        {
                            GameObject obj = pool.GetPooledTurret();
                            obj.transform.position = hit.point;
                            BuildCost?.Invoke(40);
                            obj.SetActive(true);
                        }
                    }

                }
            }
        }
    }
}
