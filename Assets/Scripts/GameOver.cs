using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [SerializeField] Button BackToMenu;
    void Start()
    {
        GameManager.Instance.GameOver = true;
        AudioManager.Instance.PlayLevelEnd();
        BackToMenu.onClick.AddListener(BackToMenuHandler);
    }

    void BackToMenuHandler()
    {
        MainMenu.GoToMainMenu();
    }

}
