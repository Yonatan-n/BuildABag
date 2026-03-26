using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BagManager : SingletonPerScene<BagManager>
{
    [SerializeField] Image layer0;
    [SerializeField] Image layer1;
    [SerializeField] Image layer2;

    [SerializeField] GameObject TrinketsParent;

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
    private Color clearWhite = new(1f, 1f, 1f, 0f);
    public ColorablePart[] GetTrinkets()
    {
        return TrinketsParent.GetComponentsInChildren<ColorablePart>().ToArray();
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
        StickersImages = TrinketsParent.GetComponentsInChildren<Image>().ToArray();
    }

    public void Previous()
    {
        _currentIndex = (_currentIndex - 1 + collection.bags.Count) % collection.bags.Count;
        SetLayers();
    }

    public void DeleteAllTrinkets()
    {
        ColorPickerManager.Instance.SelectLayer0(); // reset to bag, workaround to fix but of stickers not fading out correctly if selected
        // delete all
        for (int i = TrinketsParent.transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(TrinketsParent.transform.GetChild(i).gameObject);
        FetchStickersImages();
    }

    public void Reset()
    {
        // reset sprites
        layer0.sprite = CurrentBag.image;
        layer1.sprite = CurrentBag.image2;
        layer2.sprite = CurrentBag.image3;
        layer0.color = Color.white;
        layer1.color = layer1.sprite == null ? clearWhite : Color.white;
        layer2.color = layer2.sprite == null ? clearWhite : Color.white;
        DeleteAllTrinkets();
    }
    void SetAlpha(Image image, float alpha)
    {
        var color = image.color;
        color.a = alpha;
        image.color = color;
    }
    private void SetLayers(bool fadedOut = false)
    {
        layer0.sprite = CurrentBag.image;
        layer1.sprite = CurrentBag.image2;
        layer2.sprite = CurrentBag.image3;

        layer1.color = layer1.sprite == null ? clearWhite : Color.white;
        layer2.color = layer2.sprite == null ? clearWhite : Color.white;
        // reset colors on bag change too
        ColorPickerManager.Instance.SelectLayer0WithRandomColor();

        if (fadedOut)
        {
            SetAlpha(layer0, 0);
            SetAlpha(layer1, 0);
            SetAlpha(layer2, 0);
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