using UnityEngine;

public class ColorPickerManager : MonoBehaviour
{
    public static ColorPickerManager Instance;
    [SerializeField] ColorablePart layer0;
    [SerializeField] ColorablePart layer1;
    [SerializeField] ColorablePart layer2;
    public ColorablePart SelectedPart { get; private set; }

    [SerializeField] ColorPickerUI colorPickerUI;

    void Awake()
    {
        Instance = this;
    }
    public void SelectLayer0() => SelectPart(layer0);
    public void SelectLayer1() => SelectPart(layer1);
    public void SelectLayer2() => SelectPart(layer2);

    public void SelectPart(ColorablePart part)
    {
        SelectedPart = part;
        colorPickerUI.Open(part.GetColor());
    }

    public void ApplyColorToSelected(Color color)
    {
        SelectedPart?.ApplyColor(color);
    }
}