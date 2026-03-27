using UnityEngine;
using UnityEngine.UI;

public class TrinketClickToAdd : MonoBehaviour
{
    [SerializeField] GameObject bagParent;
    [SerializeField] GameObject TrinketOnBag;
    [SerializeField] BagTheme theme;
    [SerializeField] TrinketVisual sourceVisual;

    private Button button;
    void Start()
    {
        button = GetComponentInChildren<Button>();
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

        TrinketVisual targetVisual = trinket.GetComponent<TrinketVisual>();
        targetVisual.outline.sprite = sourceVisual.outline.sprite;

        targetVisual.colorLayer1.sprite = sourceVisual.colorLayer1.sprite;
        targetVisual.colorLayer2.sprite = sourceVisual.colorLayer2.sprite;

        targetVisual.outline.color = targetVisual.outline.sprite == null ? new Color(1, 1, 1, 0) : Color.white;
        if (targetVisual.colorLayer2.sprite == null)
        {
            targetVisual.colorLayer2.gameObject.SetActive(false);
        }

        // Apply theme to all ColorablePart components on the trinket
        foreach (var part in trinket.GetComponentsInChildren<ColorablePart>())
            part.theme = theme;

    }
}
