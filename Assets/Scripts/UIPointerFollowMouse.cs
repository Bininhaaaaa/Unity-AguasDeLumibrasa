using UnityEngine;
using UnityEngine.UI;

public class ClockPointerController : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private RectTransform ponteiro;
    [SerializeField] private RectTransform centroRelogio;
    [SerializeField] private float raioCirculo = 100f; // Raio do círculo onde o ponteiro vai orbitar

    [Header("Opções")]
    [SerializeField] private bool suavizar = true;
    [SerializeField] private float velocidadeSuavizacao = 10f;
    [SerializeField] private float distanciaMinima = 50f; // Distância mínima para considerar movimento
    [SerializeField] private float multiplicadorSensibilidade = 2f; // Aumenta sensibilidade quando perto
    [SerializeField] private bool manterPonteiroNoCirculo = true; // Se true, ponteiro fica sempre no círculo

    private Canvas canvas;
    private Camera cameraUI;

    void Start()
    {
        // Obtém o Canvas pai
        canvas = GetComponentInParent<Canvas>();

        // Define a câmera apropriada
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

        // Se não especificou o centro, usa o objeto do ponteiro como referência
        if (centroRelogio == null)
            centroRelogio = ponteiro;
    }

    void Update()
    {
        RotacionarPonteiroParaMouse();
    }

    void RotacionarPonteiroParaMouse()
    {
        // Posição do mouse na tela
        Vector2 posicaoMouse = Input.mousePosition;

        // Converte a posição do mouse para local space do pai do ponteiro
        Vector2 posicaoMouseLocal;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            ponteiro.parent as RectTransform,
            posicaoMouse,
            cameraUI,
            out posicaoMouseLocal
        );

        // Posição local do centro do relógio relativa ao pai do ponteiro
        Vector2 posicaoCentroLocal = centroRelogio.anchoredPosition;

        // Calcula a direção do centro para o mouse
        Vector2 direcao = posicaoMouseLocal - posicaoCentroLocal;

        // Calcula a distância
        float distancia = direcao.magnitude;

        // Se estiver muito perto, amplifica a direção para aumentar sensibilidade
        if (distancia < distanciaMinima && distancia > 0.1f)
        {
            float fatorAmplificacao = Mathf.Lerp(multiplicadorSensibilidade, 1f, distancia / distanciaMinima);
            direcao = direcao.normalized * (distancia * fatorAmplificacao);
        }

        // Calcula o ângulo em graus
        float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;

        // Ajusta o ângulo (Unity UI considera 0° para direita)
        // Se quiser que 0° seja para cima, subtraia 90
        angulo -= 90f;

        // Calcula a nova posição no círculo
        Vector2 novaPosicao;
        if (manterPonteiroNoCirculo)
        {
            // Mantém o ponteiro sempre no raio do círculo
            float anguloRad = (angulo + 90f) * Mathf.Deg2Rad; // Volta para radianos
            novaPosicao = posicaoCentroLocal + new Vector2(
                Mathf.Cos(anguloRad) * raioCirculo,
                Mathf.Sin(anguloRad) * raioCirculo
            );
        }
        else
        {
            // Segue o mouse até o raio máximo
            if (distancia > raioCirculo)
            {
                novaPosicao = posicaoCentroLocal + direcao.normalized * raioCirculo;
            }
            else
            {
                novaPosicao = posicaoMouseLocal;
            }
        }

        // Cria a rotação alvo
        Quaternion rotacaoAlvo = Quaternion.Euler(0, 0, angulo);

        // Ajusta velocidade de suavização baseada na distância
        float velocidadeAjustada = velocidadeSuavizacao;
        if (distancia < distanciaMinima)
        {
            // Aumenta velocidade quando está perto para resposta mais rápida
            velocidadeAjustada *= multiplicadorSensibilidade;
        }

        // Aplica a rotação e posição (com ou sem suavização)
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