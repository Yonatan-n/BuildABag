using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorPickerUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] GameObject panel;
    [SerializeField] RawImage svBox;
    [SerializeField] Slider hueSlider;
    [SerializeField] Image colorPreview;
    [SerializeField] TMP_InputField hexInput;
    [SerializeField] Button confirmButton;
    [SerializeField] Button closeButton;

    private float _hue, _sat, _val;
    private Texture2D _svTexture;
    private const int TextureSize = 128;

    void Awake()
    {
        _svTexture = new Texture2D(TextureSize, TextureSize);
        svBox.texture = _svTexture;
        // initialize
        _hue = 0f;
        _sat = 1f;
        _val = 1f;
        RegenerateSVTexture();

        hueSlider.onValueChanged.AddListener(OnHueChanged);
        hexInput.onEndEdit.AddListener(OnHexInput);
        confirmButton.onClick.AddListener(OnConfirm);
        closeButton.onClick.AddListener(Close);
        svBox.gameObject.AddComponent<SVBoxClickHandler>().OnClick = OnSVBoxClick;

    }

    public void Open(Color initialColor)
    {
        panel.SetActive(true);
        Color.RGBToHSV(initialColor, out _hue, out _sat, out _val);
        hueSlider.value = _hue;
        RegenerateSVTexture();
        UpdatePreview();
    }

    void OnHueChanged(float hue)
    {
        _hue = hue;
        RegenerateSVTexture();
        UpdatePreview();
    }

    void OnSVBoxClick(Vector2 screenPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            svBox.rectTransform,
            screenPosition,
            null,
            out Vector2 localPoint
        );

        var rect = svBox.rectTransform.rect;
        _sat = Mathf.InverseLerp(rect.xMin, rect.xMax, localPoint.x);
        _val = 1f - Mathf.InverseLerp(rect.yMin, rect.yMax, localPoint.y);

        UpdatePreview();
    }

    void OnHexInput(string hex)
    {
        if (!hex.StartsWith("#")) hex = "#" + hex;
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
        {
            Color.RGBToHSV(color, out _hue, out _sat, out _val);
            hueSlider.value = _hue;
            RegenerateSVTexture();
            UpdatePreview();
        }
    }

    void RegenerateSVTexture()
    {
        for (int y = 0; y < TextureSize; y++)
        {
            for (int x = 0; x < TextureSize; x++)
            {
                float s = (float)x / TextureSize;
                float v = (float)y / TextureSize;
                _svTexture.SetPixel(x, y, Color.HSVToRGB(_hue, s, v));
            }
        }
        _svTexture.Apply();
    }

    void UpdatePreview()
    {
        var color = Color.HSVToRGB(_hue, _sat, _val);
        colorPreview.color = color;
        hexInput.text = ColorUtility.ToHtmlStringRGB(color);
        // live preview on the bag part
        ColorPickerManager.Instance.ApplyColorToSelected(color);
    }

    void OnConfirm()
    {
        Close();
    }

    void Close()
    {
        panel.SetActive(false);
    }
}
