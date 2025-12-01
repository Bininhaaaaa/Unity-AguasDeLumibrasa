using UnityEngine;
using UnityEngine.UI;

public class CardClockPointerController : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private RectTransform ponteiro;
    [SerializeField] private RectTransform centroRelogio;
    [SerializeField] private float raioCirculo = 100f;
    [SerializeField] private float offsetRotacao = 0f;
    [Tooltip("1 = 1:1. Valor > 1 faz o ponteiro girar mais rápido que o mouse.")]
    [SerializeField] private float multiplicadorRotacao = 1f; // Nova variável

    [Header("Opções")]
    [SerializeField] private bool suavizar = true;
    [SerializeField] private float velocidadeSuavizacao = 10f;
    [SerializeField] private float distanciaMinima = 50f;
    [SerializeField] private float multiplicadorSensibilidade = 2f;
    [SerializeField] private bool manterPonteiroNoCirculo = true;

    private Canvas canvas;
    private Camera cameraUI;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            cameraUI = null;
        }
        else
        {
            cameraUI = canvas.worldCamera;
            if (cameraUI == null)
                cameraUI = Camera.main;
        }

        if (centroRelogio == null)
            centroRelogio = ponteiro;
    }

    void Update()
    {
        RotacionarPonteiroParaMouse();
    }

    void RotacionarPonteiroParaMouse()
    {
        Vector2 posicaoMouse = Input.mousePosition;

        Vector2 posicaoMouseLocal;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            ponteiro.parent as RectTransform,
            posicaoMouse,
            cameraUI,
            out posicaoMouseLocal
        );

        Vector2 posicaoCentroLocal = centroRelogio.anchoredPosition;
        Vector2 direcao = posicaoMouseLocal - posicaoCentroLocal;
        float distancia = direcao.magnitude;

        if (distancia < distanciaMinima && distancia > 0.1f)
        {
            float fatorAmplificacao = Mathf.Lerp(multiplicadorSensibilidade, 1f, distancia / distanciaMinima);
            direcao = direcao.normalized * (distancia * fatorAmplificacao);
        }

        float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;

        // Ajusta o ângulo para 0° ser o topo
        angulo -= 90f;

        // Aplica o multiplicador de rotação (sensibilidade angular)
        // Isso é feito após o ajuste de -90 para que o topo continue sendo o ponto neutro
        angulo *= multiplicadorRotacao;

        Vector2 novaPosicao;
        if (manterPonteiroNoCirculo)
        {
            // Usa o ângulo já multiplicado para calcular a posição no perímetro
            float anguloRad = (angulo + 90f) * Mathf.Deg2Rad;
            novaPosicao = posicaoCentroLocal + new Vector2(
                Mathf.Cos(anguloRad) * raioCirculo,
                Mathf.Sin(anguloRad) * raioCirculo
            );
        }
        else
        {
            // Nota: Se usar multiplicador > 1 e soltar do círculo,
            // a posição do mouse e do ponteiro vão desalinhar visualmente (comportamento esperado)
            if (distancia > raioCirculo)
            {
                // Recalcula vetor baseado no novo ângulo multiplicado
                float anguloRad = (angulo + 90f) * Mathf.Deg2Rad;
                Vector2 direcaoMultiplicada = new Vector2(Mathf.Cos(anguloRad), Mathf.Sin(anguloRad));
                novaPosicao = posicaoCentroLocal + direcaoMultiplicada * raioCirculo;
            }
            else
            {
                novaPosicao = posicaoMouseLocal;
            }
        }

        Quaternion rotacaoAlvo = Quaternion.Euler(0, 0, angulo + offsetRotacao);

        float velocidadeAjustada = velocidadeSuavizacao;
        if (distancia < distanciaMinima)
        {
            velocidadeAjustada *= multiplicadorSensibilidade;
        }

        if (suavizar)
        {
            ponteiro.rotation = Quaternion.Lerp(
                ponteiro.rotation,
                rotacaoAlvo,
                Time.deltaTime * velocidadeAjustada
            );

            ponteiro.anchoredPosition = Vector2.Lerp(
                ponteiro.anchoredPosition,
                novaPosicao,
                Time.deltaTime * velocidadeAjustada
            );
        }
        else
        {
            ponteiro.rotation = rotacaoAlvo;
            ponteiro.anchoredPosition = novaPosicao;
        }
    }
}