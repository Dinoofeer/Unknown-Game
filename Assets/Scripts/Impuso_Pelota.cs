using UnityEngine;

public class ImpulsoPelota : MonoBehaviour
{
    public float fuerza = 5f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(
                Vector2.up * fuerza,
                ForceMode2D.Impulse
            );
        }
    }
}
