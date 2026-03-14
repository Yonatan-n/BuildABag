using UnityEngine;

[CreateAssetMenu(fileName = "UITheme", menuName = "UI/Theme")]
public class UITheme : ScriptableObject
{
    [Header("Button Colors")]
    public Color buttonNormal = Color.white;
    public Color buttonHighlight = new(0.9f, 0.9f, 0.9f);
    public Color buttonPressed = new(0.7f, 0.7f, 0.7f);
    public Color buttonDisabled = new(0.5f, 0.5f, 0.5f);
    public Color buttonSelected = new(0.8f, 0.8f, 0.8f);

    [Header("Text Colors")]
    public Color buttonTextColor = Color.black;
    [Header("Panel Colors")]
    public Color PanelBackground = Color.black;
}