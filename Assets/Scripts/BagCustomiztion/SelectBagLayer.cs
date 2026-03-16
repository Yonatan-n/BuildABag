using UnityEngine;
using UnityEngine.UI;

public class SelectBagLayer : MonoBehaviour
{
    [SerializeField] Button reset;
    void Start()
    {
        reset.onClick.AddListener(OnResetButtonClicked);
        // set to layer0 on start
        ColorPickerManager.Instance.SelectLayer0();
    }

    void OnResetButtonClicked()
    {
        Debug.Log("inital reset");
        ConfirmDialog.Show("Reset bag?", "All Changes will be lost forever.", onConfirm: BagManager.Instance.Reset);
    }
}
