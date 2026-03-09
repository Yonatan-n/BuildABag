using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SliderPointerUp : MonoBehaviour, IPointerUpHandler
{
    public UnityEvent onPointerUp;

    public void OnPointerUp(PointerEventData eventData)
    {
        onPointerUp?.Invoke();
    }
}