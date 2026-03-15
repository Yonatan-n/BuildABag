using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BagManager : SingletonPerScene<BagManager>
{
    [SerializeField] Image layer0;
    [SerializeField] GameObject LayersParent;

    [SerializeField] BagCollection collection;
    public ColorPickerUI colorPicker;

    [Header("Buttons")]
    [SerializeField] Button Left;
    [SerializeField] Button Right;
    [SerializeField] Button ResetButton;
    private int _currentIndex = 0;
    public Image[] StickersImages;
    public BagModel CurrentBag => collection.bags[_currentIndex];
    public Color GetLayer0Color() => layer0.color;

    public ColorablePart[] GetTrinkets()
    {
        Debug.Log($"BagManager layer0: {layer0.gameObject.name} | color: #{ColorUtility.ToHtmlStringRGB(layer0.color)}");

        return LayersParent.GetComponentsInChildren<ColorablePart>().Skip(1).ToArray();
    }

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
    private void FetchStickersImages()
    {
        StickersImages = LayersParent.GetComponentsInChildren<Image>().Skip(1).ToArray();
    }

    public void Previous()
    {
        _currentIndex = (_currentIndex - 1 + collection.bags.Count) % collection.bags.Count;
        SetLayers();
    }

    public void DeleteAllTrinkets()
    {
        ColorPickerManager.Instance.SelectLayer0(); // reset to bag, workaround to fix but of stickers not fading out correctly if selected
        if (LayersParent.transform.childCount <= 1) return;
        // delete all but the first
        for (int i = LayersParent.transform.childCount - 1; i >= 1; i--)
        {
            DestroyImmediate(LayersParent.transform.GetChild(i).gameObject);
        }
        // ColorPickerManager.Instance.ClearSelection();
        FetchStickersImages();
    }

    public void Reset()
    {
        // reset sprites
        layer0.sprite = CurrentBag.image;
        layer0.color = Color.white;
        DeleteAllTrinkets();
    }

    private void SetLayers(bool fadedOut = false)
    {
        layer0.sprite = CurrentBag.image;
        // reset colors on bag change too
        ColorPickerManager.Instance.SelectLayer0WithRandomColor();

        if (fadedOut)
        {
            var _color = layer0.color;
            _color.a = 0;
            layer0.color = _color;
            return;
        }
        // reset trinkets for now
    }

    void Start()
    {
        SetLayers();
    }

    void OnEnable()
    {
        Left.onClick.AddListener(Previous);
        Right.onClick.AddListener(Next);
    }

    void OnDisable()
    {
        Left.onClick.RemoveListener(Previous);
        Right.onClick.RemoveListener(Next);
    }

}


// TODO:
// DONE stickers drag and drop
// DONE get request note
// DONE submit
// DONE score
// history
// next level (new sound, new background, new bags