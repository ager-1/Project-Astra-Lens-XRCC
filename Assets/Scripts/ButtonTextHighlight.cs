using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; // or using UnityEngine.UI if using legacy Text

public class ButtonTextHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI buttonText; // or Text for legacy UI
    public Color normalColor = Color.white;
    public Color highlightColor = Color.yellow;

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.color = highlightColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.color = normalColor;
    }
}