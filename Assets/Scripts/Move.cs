using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class RagePlayerController : MonoBehaviour // <-- Nombre cambiado para no causar conflictos
{
    [Header("Movimiento Base")]
    [SerializeField] private float velocidadMovimiento = 8f;

    [Header("Configuración de Salto")]
    [SerializeField] private float alturaSalto = 3f;
    [SerializeField] private int saltosMaximos = 2;
    private int saltosRestantes;

    [Header("Sensación del Salto (Rage Game)")]
    [SerializeField] private float multiplicadorCaida = 2.5f;
    [SerializeField] private float multiplicadorSaltoCorto = 2f;

    [Header("Detección de Suelo")]
    [SerializeField] private Transform puntoSuelo;
    [SerializeField] private Vector2 tamanoCajaSuelo = new Vector2(0.8f, 0.2f);
    [SerializeField] private LayerMask capaSuelo;

    private Rigidbody2D rb;
    private float inputHorizontal;
    private bool enSuelo;
    private bool intentarSalto;
    private bool cancelarSalto;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        inputHorizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            intentarSalto = true;
        }

        if (Input.GetButtonUp("Jump"))
        {
            cancelarSalto = true;
        }
    }

    private void FixedUpdate()
    {
        VerificarSuelo();
        Mover();
        AplicarGravedadDinamica();
        ManejarSalto();
    }

    private void Mover()
    {
        rb.linearVelocity = new Vector2(inputHorizontal * velocidadMovimiento, rb.linearVelocity.y);
    }

    private void ManejarSalto()
    {
        if (intentarSalto)
        {
            if (saltosRestantes > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, VelocidadParaAltura(alturaSalto));
                saltosRestantes--;
            }
            intentarSalto = false;
        }
    }

    private void AplicarGravedadDinamica()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (multiplicadorCaida - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0 && cancelarSalto)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (multiplicadorSaltoCorto - 1) * Time.fixedDeltaTime;
            cancelarSalto = false;
        }
    }

    private void VerificarSuelo()
    {
        bool estadoAnteriorSuelo = enSuelo;
        enSuelo = Physics2D.OverlapBox(puntoSuelo.position, tamanoCajaSuelo, 0f, capaSuelo);

        if (enSuelo)
        {
            saltosRestantes = saltosMaximos;
        }
        else if (estadoAnteriorSuelo && saltosRestantes == saltosMaximos)
        {
            saltosRestantes--;
        }
    }

    private float VelocidadParaAltura(float alturaDeseada)
    {
        float gravedad = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
        return Mathf.Sqrt(2f * gravedad * alturaDeseada);
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