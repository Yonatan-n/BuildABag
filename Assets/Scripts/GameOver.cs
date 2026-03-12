using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [SerializeField] Button BackToMenu;
    [SerializeField] TextMeshProUGUI BagsText;
    [SerializeField] TextMeshProUGUI MoneyText;
    void Start()
    {
        GameManager.Instance.GameOver = true;
        AudioManager.Instance.PlayLevelEnd();
        BackToMenu.onClick.AddListener(BackToMenuHandler);
        MoneyText.text = $"Money Earned {Score.Instance.TotalMoney}$";
        BagsText.text = $"Bags Made: {Score.Instance.BagCount}";
    }

    void BackToMenuHandler()
    {
        MainMenu.GoToMainMenu();
    }

}
