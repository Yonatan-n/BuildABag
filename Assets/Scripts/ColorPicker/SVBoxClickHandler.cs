using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class SVBoxClickHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IDragHandler

{
    public Action<Vector2> OnClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke(eventData.position);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnClick?.Invoke(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnClick?.Invoke(eventData.position);
    }
}