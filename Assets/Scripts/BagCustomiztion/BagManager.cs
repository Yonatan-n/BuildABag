using UnityEngine;
using UnityEngine.UI;

public class BagManager : MonoBehaviour
{

    [SerializeField] Image layer0;
    [SerializeField] Image layer1;
    [SerializeField] Image layer2;
    [SerializeField] BagCollection collection;
    [Header("Buttons")]
    [SerializeField] Button Left;
    [SerializeField] Button Right;
    private int _currentIndex = 0;

    public BagModel CurrentBag => collection.bags[_currentIndex];

    public void Next()
    {
        _currentIndex = (_currentIndex + 1) % collection.bags.Count;
        SetLayers();

    }

    private void SetLayers()
    {
        var firstVariant = CurrentBag.variants[0];
        layer0.sprite = firstVariant.baseLayer;
        layer1.sprite = firstVariant.accentLayer;
        layer2.sprite = firstVariant.zipperLayer;
    }

    public void Previous()
    {
        _currentIndex = (_currentIndex - 1 + collection.bags.Count) % collection.bags.Count;
        SetLayers();
    }

    void LeftHandler()
    {
        Previous();
    }

    void RightHandler()
    {
        Next();
    }

    void Start()
    {
        Left.onClick.AddListener(LeftHandler);
        Right.onClick.AddListener(RightHandler);
        SetLayers(); // init
    }

}
