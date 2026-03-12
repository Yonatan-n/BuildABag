using UnityEngine;
using UnityEngine.UI;

public class BagManager : Singleton<BagManager>
{
    [SerializeField] Image layer0;
    [SerializeField] Image layer1;
    [SerializeField] Image layer2;
    [SerializeField] BagCollection collection;

    [Header("Buttons")]
    [SerializeField] Button Left;
    [SerializeField] Button Right;
    [SerializeField] Button ResetButton;

    private int _currentIndex = 0;
    private BagVariant _currentVariant;

    public BagModel CurrentBag => collection.bags[_currentIndex];

    public void Next()
    {
        _currentIndex = (_currentIndex + 1) % collection.bags.Count;
        SetLayers();
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

    private void SetLayers()
    {
        _currentVariant = CurrentBag.variants[0]; // default to first variant
        layer0.sprite = _currentVariant.baseLayer;
        layer1.sprite = _currentVariant.accentLayer;
        layer2.sprite = _currentVariant.zipperLayer;

        // reset colors on bag change too
        layer0.color = Color.white;
        layer1.color = Color.white;
        layer2.color = Color.white;
    }

    void Start()
    {
        Left.onClick.AddListener(Previous);
        Right.onClick.AddListener(Next);
        SetLayers();
    }

}


// TODO:
// stickers drag and drop
// get request note
// submit
// score
// history
// timer
// next level (new sound, new background, new bags