using UnityEngine;
using UnityEngine.UI;

public class TrinketClickToAdd : MonoBehaviour
{
    [SerializeField] GameObject bagParent;
    [SerializeField] GameObject TrinketOnBag;
    [SerializeField] BagTheme theme;

    private Button button;
    private Image image;
    void Start()
    {
        button = GetComponentInChildren<Button>();
        image = GetComponentInChildren<Image>();
        button.onClick.AddListener(OnClickHandler);
    }
    void OnClickHandler()
    {
        // Get the bag's RectTransform to know its size
        RectTransform bagRect = bagParent.GetComponent<RectTransform>();
        // quarter to have it always around the center, not too much far out
        float quarterW = bagRect.rect.width / 4f;
        float quarterH = bagRect.rect.height / 4f;

        // Instantiate at zero first, then set anchored position
        GameObject trinket = Instantiate(TrinketOnBag, bagParent.transform);
        RectTransform trinketRect = trinket.GetComponent<RectTransform>();

        trinketRect.anchoredPosition = new Vector2(
            Random.Range(-quarterW, quarterW),
            Random.Range(-quarterH, quarterH)
        );

        trinket.GetComponent<Image>().sprite = image.sprite;
        trinket.GetComponent<ColorablePart>().theme = theme;
    }
}
