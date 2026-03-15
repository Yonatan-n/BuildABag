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
        Debug.Log($"SelectPart: {part.gameObject.name}", part.gameObject);
        SelectedPart = part;
        colorPickerUI.Open(part.GetColor());
    }

    public void ApplyColorToSelected(Color color)
    {
        if (SelectedPart == null) return;
        Debug.Log($"ApplyColorToSelected called on: {SelectedPart.gameObject.name}", SelectedPart.gameObject);
        SelectedPart.ApplyColor(color);
    }
}