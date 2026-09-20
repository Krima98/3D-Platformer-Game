using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonEvent : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    // --- Hover --- //
    public void OnPointerEnter(PointerEventData e)
    {
        AudioManager.Instance.PlayHoverSound();
    }

    // --- click --- //
    public void OnPointerClick(PointerEventData e)
    {
        AudioManager.Instance.PlayClickSound();
    }
}
