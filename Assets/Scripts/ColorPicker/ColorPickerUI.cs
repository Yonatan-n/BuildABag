using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ColorPickerUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] GameObject panel;
    [SerializeField] RawImage svBox;
    [SerializeField] Slider hueSlider;
    [SerializeField] Image hueSliderBackground;
    [SerializeField] Image colorPreview;
    [SerializeField] TMP_InputField hexInput;
    [SerializeField] Button confirmButton;
    [SerializeField] Button closeButton;

    private float _hue, _sat, _val;
    private Texture2D _svTexture;
    private const int TextureSize = 128;

    void Start()
    {
        _svTexture = new Texture2D(TextureSize, TextureSize);
        svBox.texture = _svTexture;


        hueSlider.onValueChanged.AddListener(OnHueChanged);
        hexInput.onEndEdit.AddListener(OnHexInput);
        confirmButton.onClick.AddListener(OnConfirm); // maybe remove
        closeButton.onClick.AddListener(Close); // maybe remove
        svBox.gameObject.AddComponent<SVBoxClickHandler>().OnClick = OnSVBoxClick;

        GenerateHueSliderBackground();
        _hue = 0f;
        _sat = 1f;
        _val = 1f;
        RegenerateSVTexture();
        UpdatePreview(); // to set initial values
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
        _val = Mathf.InverseLerp(rect.yMin, rect.yMax, localPoint.y);

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
        if (_svTexture == null) _svTexture = new Texture2D(256, 256, TextureFormat.RGB24, false);

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

    void GenerateHueSliderBackground()
    {
        int width = 256;
        int height = 1;
        Texture2D hueTexture = new(width, height);

        for (int x = 0; x < width; x++)
        {
            float h = (float)x / width;
            hueTexture.SetPixel(x, 0, Color.HSVToRGB(h, 1f, 1f));
        }
        hueTexture.Apply();

        hueSliderBackground.sprite = Sprite.Create(
            hueTexture,
            new Rect(0, 0, width, height),
            new Vector2(0.5f, 0.5f)
        );
    }

    void UpdatePreview()
    {
        var color = Color.HSVToRGB(_hue, _sat, _val);
        colorPreview.color = color;
        hueSlider.handleRect.GetComponent<Image>().color = color;
        hexInput.text = ColorUtility.ToHtmlStringRGB(color);
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
