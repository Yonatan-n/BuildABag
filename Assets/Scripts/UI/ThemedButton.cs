// ThemedButton.cs
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ThemedButton : MonoBehaviour
{
    public UITheme theme;
    [SerializeField] bool IgnoreBaseColor;
    private bool _isSelected = false;

    void Start() => ApplyTheme();

    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        ApplyTheme();
    }

    public void ApplyTheme()
    {
        if (theme == null) return;
        var btn = GetComponent<Button>();

        if (TryGetComponent<Image>(out var image)) image.color = Color.white;

        var colors = btn.colors;
        if (!IgnoreBaseColor) colors.normalColor = _isSelected ? theme.buttonSelected : theme.buttonNormal;
        colors.highlightedColor = _isSelected ? theme.buttonSelected : theme.buttonHighlight;
        colors.highlightedColor = theme.buttonHighlight;
        colors.pressedColor = theme.buttonPressed;
        colors.disabledColor = theme.buttonDisabled;
        colors.selectedColor = theme.buttonSelected;
        colors.colorMultiplier = 1f;

        btn.colors = colors;
        var label = GetComponentInChildren<Text>();
        if (label != null && !IgnoreBaseColor) label.color = theme.buttonTextColor;
    }
}