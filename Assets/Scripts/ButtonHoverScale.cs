using UnityEngine;
using UnityEngine.EventSystems;

// Add this script to your Button GameObject
public class ButtonHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // How much to scale the button on hover (e.g., 1.1 = 110%)
    public float hoverScale = 1.1f;

    private Vector3 originalScale;

    void Start()
    {
        // Store the button's starting scale
        originalScale = transform.localScale;
    }

    // Called when the mouse pointer enters the button
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = originalScale * hoverScale;
    }

    // Called when the mouse pointer exits the button
    public void OnPointerExit(PointerEventData eventData)
    {
        // Reset the scale
        transform.localScale = originalScale;
    }
}