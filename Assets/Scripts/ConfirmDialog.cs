using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ConfirmDialog : MonoBehaviour
{
    public static ConfirmDialog Instance;

    [SerializeField] GameObject panel;
    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text messageText;
    [SerializeField] Button confirmButton;
    [SerializeField] Button cancelButton;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public static void Show(string title, string message, Action onConfirm)
    {
        Instance.titleText.text = title;
        Instance.messageText.text = message;
        Instance.panel.SetActive(true);

        Instance.confirmButton.onClick.RemoveAllListeners();
        Instance.confirmButton.onClick.AddListener(() =>
        {
            onConfirm?.Invoke();
            Instance.panel.SetActive(false);
        });

        Instance.cancelButton.onClick.RemoveAllListeners();
        Instance.cancelButton.onClick.AddListener(() =>
            Instance.panel.SetActive(false));
    }
}
