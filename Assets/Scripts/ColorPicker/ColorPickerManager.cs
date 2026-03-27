using UnityEngine;

public class ColorPickerManager : MonoBehaviour
{
    public static ColorPickerManager Instance;
    [SerializeField] ColorablePart layer0;
    public ColorablePart SelectedPart { get; private set; }

    [SerializeField] ColorPickerUI colorPickerUI;

    void Awake()
    {
        Instance = this;
    }
    public void SelectLayer0()
    {
        SelectPart(layer0);
    }

    public void SelectPart(ColorablePart part)
    {
        SelectedPart = part;
        colorPickerUI.Open(part.GetColor());
    }

    public void ApplyColorToSelected(Color color)
    {
        if (SelectedPart == null) return;
        SelectedPart.ApplyColor(color);
    }

    public void SelectLayer0WithRandomColor()
    {
        SelectedPart = layer0;
        Color randomColor = colorPickerUI.GetRandomColor();
        layer0.ApplyColor(randomColor);
        colorPickerUI.Open(randomColor);
    }
}