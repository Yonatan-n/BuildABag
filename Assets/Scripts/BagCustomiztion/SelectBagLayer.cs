using UnityEngine;
using UnityEngine.UI;

public class SelectBagLayer : MonoBehaviour
{
    [SerializeField] Button layer0;
    [SerializeField] Button layer1;
    [SerializeField] Button layer2;
    void Start()
    {
        layer0.onClick.AddListener(ColorPickerManager.Instance.SelectLayer0);
        layer1.onClick.AddListener(ColorPickerManager.Instance.SelectLayer1);
        layer2.onClick.AddListener(ColorPickerManager.Instance.SelectLayer2);
    }
}
