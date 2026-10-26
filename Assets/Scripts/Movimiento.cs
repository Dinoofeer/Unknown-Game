using UnityEngine;


public class Movimiento : MonoBehaviour
{
    public float velocidad = 5f;

    void Update()
    {
        Vector3 direccion = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) direccion += Vector3.up;
        if (Input.GetKey(KeyCode.S)) direccion += Vector3.down;
        if (Input.GetKey(KeyCode.A)) direccion += Vector3.left;
        if (Input.GetKey(KeyCode.D)) direccion += Vector3.right;

  
        transform.Translate(direccion.normalized * velocidad * Time.deltaTime);
    }
}
