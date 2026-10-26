using UnityEngine;

// Mejora C: avisa en la consola cuando la pelota toca el suelo.
// Va en la pelota. El objeto del suelo debe llamarse "Suelo" en la Hierarchy.
public class DetectarSuelo : MonoBehaviour
{
    public string nombreSuelo = "Suelo";
    private int caidas = 0;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == nombreSuelo)
        {
            caidas++;
            Debug.Log("La pelota toco el suelo. Caidas: " + caidas);
        }
    }
}
