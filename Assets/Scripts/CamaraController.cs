using UnityEngine;

public class CamaraSeguimiento : MonoBehaviour
{
    [Header("Objetivo")]
    [SerializeField] private Transform objetivo;
    [SerializeField] private Vector2 desplazamiento = new Vector2(0f, 1f);

    [Header("Suavizado")]
    [SerializeField] private float tiempoSuavizado = 0.15f;

    [Header("Límites del nivel (opcional)")]
    [SerializeField] private bool usarLimites = false;
    [SerializeField] private Vector2 limiteMinimo = new Vector2(-50f, -10f);
    [SerializeField] private Vector2 limiteMaximo = new Vector2(50f, 20f);

    private Vector3 velocidadActual;
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    // LateUpdate: se ejecuta después de que el jugador se movió, evita temblores
    private void LateUpdate()
    {
        if (objetivo == null) return;

        Vector3 destino = new Vector3(
            objetivo.position.x + desplazamiento.x,
            objetivo.position.y + desplazamiento.y,
            transform.position.z);

        Vector3 posicion = Vector3.SmoothDamp(transform.position, destino, ref velocidadActual, tiempoSuavizado);

        if (usarLimites && cam != null && cam.orthographic)
        {
            float mitadAlto = cam.orthographicSize;
            float mitadAncho = mitadAlto * cam.aspect;

            posicion.x = Mathf.Clamp(posicion.x, limiteMinimo.x + mitadAncho, limiteMaximo.x - mitadAncho);
            posicion.y = Mathf.Clamp(posicion.y, limiteMinimo.y + mitadAlto, limiteMaximo.y - mitadAlto);
        }

        transform.position = posicion;
    }

    private void OnDrawGizmosSelected()
    {
        if (!usarLimites) return;

        Gizmos.color = Color.yellow;
        Vector2 centro = (limiteMinimo + limiteMaximo) / 2f;
        Vector2 tamano = limiteMaximo - limiteMinimo;
        Gizmos.DrawWireCube(centro, tamano);
    }
}