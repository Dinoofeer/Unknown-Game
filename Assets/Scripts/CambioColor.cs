using UnityEngine;

public class CambioColor : MonoBehaviour
{
    public KeyCode tecla = KeyCode.C;
    public Color[] colores = { Color.white, Color.red, Color.green, Color.cyan, Color.yellow };

    private SpriteRenderer sr;
    private int indice = 0;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(tecla))
        {
            indice = (indice + 1) % colores.Length;
            sr.color = colores[indice];
        }
    }
}
