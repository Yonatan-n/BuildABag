using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorablePart : MonoBehaviour, IPointerClickHandler
{
    public string partId;
    private Image _image;
    private Color defaultColor = Color.white;

    void Awake()
    {
        _image = GetComponent<Image>();
        _image.color = defaultColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ColorPickerManager.Instance.SelectPart(this);
    }

    public Color GetColor() => _image.color;

    public void ApplyColor(Color color)
    {
        _image.color = color;
    }
}