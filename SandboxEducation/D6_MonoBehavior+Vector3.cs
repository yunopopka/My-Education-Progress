using UnityEditor;
using UnityEngine;

public class CubeEngine : MonoBehaviour
{
    private float Currentspeed = 5f;
    public float turnSpeed = 10f;
    public float normalSpeed = 5f;
    public float sprintSpeed = 12f;
    private float heat = 0f;
    private bool isOverheated = false;
    private void Awake()
    {
        
    }

    private void Start()
    {
        Currentspeed = normalSpeed;
        Debug.Log("2. Start: Я готов к бою!");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Mathf.Abs(transform.position.x) <= 3f && Mathf.Abs(transform.position.z) <= 3f) 
            {
                GameManager.Instance.AddScore(10);
            }
            else { Debug.Log("Here is not anomalies"); }
        }


        // 1. ПРОВЕРКА СОСТОЯНИЯ: Мы сломаны?
        if (isOverheated)
        {
            // Танк сломан! Стоим и остываем.
            Currentspeed = 0f;
            heat -= 10f * Time.deltaTime; // Остываем

            if (heat <= 0f) // Как только остыли до конца...
            {
                isOverheated = false; // Починились!
                Debug.Log("Двигатель остыл! Можно ехать.");
            }
        }
        else
        {
            // 2. МЫ В НОРМЕ. Проверяем, жмет ли игрок кнопку газа
            if (Input.GetKey(KeyCode.LeftShift))
            {
                // Игрок жмет газ
                Currentspeed = sprintSpeed;
                heat += 20f * Time.deltaTime; // Греемся!

                if (heat >= 100f) // Как только нагрелись до предела...
                {
                    isOverheated = true; // Ломаемся!
                    Debug.Log("ПЕРЕГРЕВ! Двигатель заглох.");
                }
            }
            else
            {
                // Игрок отпустил газ
                Currentspeed = normalSpeed;
                heat -= 10f * Time.deltaTime; // Остываем на ходу
            }
        }

        // 3. Защита от выхода за рамки (один Clamp на все случаи жизни)
        heat = Mathf.Clamp(heat, 0f, 100f);
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(x, 0, z);


        transform.position += direction * Currentspeed * Time.deltaTime;
        float clamedX = Mathf.Clamp(transform.position.x, -10f, 10f);
        float clamedZ = Mathf.Clamp(transform.position.z, -10f, 10f);
        transform.position = new Vector3(clamedX, transform.position.y, clamedZ);

        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, turnSpeed * Time.deltaTime);
        }

    }

    private void FixedUpdate()
    {

    }

}
