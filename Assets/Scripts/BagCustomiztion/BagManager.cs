using UnityEngine;
using UnityEngine.UI;

public class BagManager : SingletonPerScene<BagManager>
{
    [SerializeField] Image layer0;
    [SerializeField] Image layer1;
    [SerializeField] Image layer2;
    [SerializeField] BagCollection collection;
    [SerializeField] ColorPickerUI colorPicker;

    [Header("Buttons")]
    [SerializeField] Button Left;
    [SerializeField] Button Right;
    [SerializeField] Button ResetButton;
    private int _currentIndex = 0;
    private BagVariant _currentVariant;

    public BagModel CurrentBag => collection.bags[_currentIndex];
    Color ClearWhite = new Color(1f, 1f, 1f, 0f);

    public void Next()
    {
        _currentIndex = (_currentIndex + 1) % collection.bags.Count;
        SetLayers();
    }
    public void LoadNewBagFadedOut()
    {
        _currentIndex = Random.Range(0, collection.bags.Count);
        SetLayers(fadedOut: true);
    }

    public void Previous()
    {
        _currentIndex = (_currentIndex - 1 + collection.bags.Count) % collection.bags.Count;
        SetLayers();
    }

    public void ApplyVariant(BagVariant variant)
    {
        _currentVariant = variant;
        SetLayers();
    }

    public void Reset()
    {
        // reset sprites
        layer0.sprite = _currentVariant.baseLayer;
        layer1.sprite = _currentVariant.accentLayer;
        layer2.sprite = _currentVariant.zipperLayer;

        // reset colors
        layer0.color = Color.white;
        layer1.color = Color.white;
        layer2.color = Color.white;
    }

    private void SetLayers(bool fadedOut = false)
    {
        _currentVariant = CurrentBag.variants[0]; // default to first variant
        layer0.sprite = _currentVariant.baseLayer;
        layer1.sprite = _currentVariant.accentLayer;
        layer2.sprite = _currentVariant.zipperLayer;
        // reset colors on bag change too
        colorPicker.SetRandomColor();
        if (fadedOut)
        {
            var _color = layer0.color;
            _color.a = 0;
            layer0.color = _color;
            layer1.color = ClearWhite;
            layer2.color = ClearWhite;
            return;
        }
        // reset trinkets for now
        layer1.color = Color.white;
        layer2.color = Color.white;
    }

    void OnEnable()
    {
        Left.onClick.AddListener(Previous);
        Right.onClick.AddListener(Next);
        SetLayers();
    }

    void OnDisable()
    {
        Left.onClick.RemoveListener(Previous);
        Right.onClick.RemoveListener(Next);
    }

}


// TODO:
// stickers drag and drop
// get request note
// submit
// score
// history
// next level (new sound, new background, new bags