using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class SVBoxClickHandler : MonoBehaviour, IPointerClickHandler
{
    public Action<Vector2> OnClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke(eventData.position);
    }
}