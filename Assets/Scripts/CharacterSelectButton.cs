using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour
{
    [SerializeField] private Button characterButton;
    [SerializeField] private Button selectButton;

    [SerializeField] private Image targetImage;       // imagem que vai trocar o sprite
    [SerializeField] private Sprite selectedSprite;   // sprite final
    [SerializeField] private float scaleUp = 1.15f;   // quanto aumenta
    [SerializeField] private float duration = 0.15f;  // velocidade da animação

    public static Action OnCharacterSelected;

    private bool isSelected = false;

    void Awake()
    {
        characterButton.onClick.AddListener(SwitchState);
        selectButton.onClick.AddListener(SelectCharacter);
    }

    private void SelectCharacter()
    {
        if (!isSelected) return;

        Debug.Log("Character confirmed!");
        OnCharacterSelected?.Invoke();
        // aqui você pode adicionar lógica adicional para confirmar a seleção do personagem
    }

    private void SwitchState()
    {
        if (isSelected) return;

        StartCoroutine(AnimateAndSelect());
        Debug.Log("Personagem selecionado!");

    }

    private System.Collections.IEnumerator AnimateAndSelect()
    {
        isSelected = true;
        characterButton.interactable = false;

        Vector3 initialScale = transform.localScale;
        Vector3 finalScale = initialScale * scaleUp;
        float t = 0f;

        // animação simples de scale
        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = t / duration;
            transform.localScale = Vector3.Lerp(initialScale, finalScale, normalized);
            yield return null;
        }

        transform.localScale = finalScale;

        // troca o sprite quando a animação acabar
        if (targetImage != null && selectedSprite != null)
        {
            targetImage.sprite = selectedSprite;
            transform.localScale = finalScale * 1.15f;
        }

    }
}
