using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento Base")]
    [SerializeField] private float velocidadMovimiento = 8f;

    [Header("Saltos (alturas en unidades de Unity)")]
    [Tooltip("Altura máxima del primer salto manteniendo el botón.")]
    [SerializeField] private float alturaSalto = 2.5f;
    [Tooltip("Altura máxima del salto en el aire (doble salto).")]
    [SerializeField] private float alturaSegundoSalto = 2f;
    [Tooltip("Total de saltos permitidos antes de tocar el suelo (2 = doble salto).")]
    [SerializeField] private int saltosMaximos = 2;

    [Header("Sensación del Salto (Rage Game)")]
    [SerializeField] private float multiplicadorCaida = 2.5f;
    [SerializeField] private float multiplicadorSaltoCorto = 2f;

    [Header("Tolerancia de Salto")]
    [Tooltip("Tiempo después de dejar el suelo en el que aún se puede saltar.")]
    [SerializeField] private float tiempoCoyote = 0.1f;
    [Tooltip("Tiempo que se recuerda un salto presionado antes de aterrizar.")]
    [SerializeField] private float tiempoBuffer = 0.1f;

    [Header("Detección de Suelo")]
    [SerializeField] private Transform puntoSuelo;
    [SerializeField] private Vector2 tamanoCajaSuelo = new Vector2(0.8f, 0.2f);
    [SerializeField] private LayerMask capaSuelo;

    private Rigidbody2D rb;
    private float inputHorizontal;
    private bool enSuelo;
    private bool saltoMantenido;
    private float contadorCoyote;
    private float contadorBuffer;
    private int saltosRestantes;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        saltosRestantes = saltosMaximos;

        if (puntoSuelo == null)
        {
            Debug.LogError("PlayerController: falta asignar 'puntoSuelo' en el Inspector.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        // Inputs (Input Manager clásico). GetAxisRaw elimina la inercia.
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        saltoMantenido = Input.GetButton("Jump");

        // Jump buffer: recordamos la pulsación durante un instante
        if (Input.GetButtonDown("Jump"))
            contadorBuffer = tiempoBuffer;
        else
            contadorBuffer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        VerificarSuelo();
        ActualizarContadores();

        Mover();
        ManejarSalto();
        AplicarGravedadDinamica();
    }

    private void ActualizarContadores()
    {
        // Recargamos solo si está en el suelo y NO subiendo,
        // para no recargar en el frame justo después de saltar.
        if (enSuelo && rb.linearVelocity.y <= 0.01f)
        {
            contadorCoyote = tiempoCoyote;
            saltosRestantes = saltosMaximos;
        }
        else
        {
            contadorCoyote -= Time.fixedDeltaTime;

            // Si cayó de una plataforma sin saltar, perdió el salto "de suelo":
            // solo le quedan los saltos aéreos.
            if (contadorCoyote <= 0f && saltosRestantes == saltosMaximos)
                saltosRestantes = saltosMaximos - 1;
        }
    }

    private void Mover()
    {
        // Control horizontal absoluto, sin resbalar.
        // Nota: si agregas knockback o plataformas móviles, habrá que modificar esto.
        rb.linearVelocity = new Vector2(inputHorizontal * velocidadMovimiento, rb.linearVelocity.y);
    }

    private void ManejarSalto()
    {
        if (contadorBuffer <= 0f) return;

        if (contadorCoyote > 0f)
        {
            // Salto desde el suelo (o dentro del coyote time)
            Saltar(alturaSalto);
            saltosRestantes = saltosMaximos - 1;
            contadorCoyote = 0f;
        }
        else if (saltosRestantes > 0)
        {
            // Salto en el aire: sobrescribe la velocidad vertical (incluida la de caída)
            Saltar(alturaSegundoSalto);
            saltosRestantes--;
        }
    }

    private void Saltar(float altura)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, VelocidadParaAltura(altura));
        contadorBuffer = 0f;
    }

    // v = sqrt(2 * g * h): velocidad inicial necesaria para alcanzar una altura exacta
    private float VelocidadParaAltura(float altura)
    {
        float gravedad = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
        return Mathf.Sqrt(2f * gravedad * altura);
    }

    private void AplicarGravedadDinamica()
    {
        if (rb.linearVelocity.y < 0f)
        {
            // Caída más rápida para un aterrizaje seco
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (multiplicadorCaida - 1f) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0f && !saltoMantenido)
        {
            // Salto de altura variable: si sueltas el botón, el salto se acorta
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (multiplicadorSaltoCorto - 1f) * Time.fixedDeltaTime;
        }
    }

    private void VerificarSuelo()
    {
        enSuelo = Physics2D.OverlapBox(puntoSuelo.position, tamanoCajaSuelo, 0f, capaSuelo);
    }

    private void OnDrawGizmosSelected()
    {
        if (puntoSuelo != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(puntoSuelo.position, tamanoCajaSuelo);
        }
    }
}