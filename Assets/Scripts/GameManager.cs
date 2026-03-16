using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ColorName
{
    Red, Orange, Yellow, Green, Cyan, Blue, Purple, Pink,
    Black, White, Gray
}

public struct ColorRange
{
    public Vector2 hueRange;
    public Vector2 satRange; // 0 = grey/white/black, 1 = fully saturated
    public Vector2 valRange; // 0 = black, 1 = white

    public readonly string ToHex()
    {
        float midHue = (hueRange.x + hueRange.y) / 2f;
        float midSat = (satRange.x + satRange.y) / 2f;
        float midVal = (valRange.x + valRange.y) / 2f;
        Color c = Color.HSVToRGB(midHue, midSat, midVal);
        return "#" + ColorUtility.ToHtmlStringRGB(c);
    }
}


public class GameManager : SingletonPerScene<GameManager>
{

    [SerializeField] Button Submit;
    [SerializeField] Button Next;
    [SerializeField] Button Prev;
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] TextMeshProUGUI NoteCustomer;
    [SerializeField] ParticleSystem confettiParticles;
    [SerializeField] GameObject LayersGroup;


    int TotalMoney;
    List<OrderRequest> orderRequests;
    OrderRequest currentOrderRequest;
    int orderRequestIndex = 0;
    public bool GameOver = false;
    bool isFirstOrder = true;

    void Init()
    {
        orderRequests = new List<OrderRequest>()
        {
            // goth 
            new()
            {
                customerName = "Astrid",
                loveTheme = BagTheme.Goth,
                loveColorRange = ColorRanges.Blue,
                loveFluff = new[] { "death", "ravens" },
                hateTheme = null,
                hateColorRange = ColorRanges.Yellow,
                hateFluff = new[] { "mornings" },
            },
            new()
            {
                customerName = "Lilith",
                loveTheme = BagTheme.Goth,
                loveColorRange = ColorRanges.Purple,
                loveFluff = new[] { "darkness", "candles" },
                hateTheme = BagTheme.Girly,
                hateColorRange = ColorRanges.Green,
                hateFluff = new[] { "apples", "sheep" },
            },
            new()
            {
                customerName = "Bianca",
                loveTheme = BagTheme.Goth,
                loveColorRange = ColorRanges.Grey,
                loveFluff = new[] { "skulls", "bones" },
                hateTheme = BagTheme.Girly,
                hateColorRange = ColorRanges.Pink,
                hateFluff = new[] { "glitter", "rainbows" },
            },
            new()
            {
                customerName = "Elsa",
                loveTheme = BagTheme.Goth,
                loveColorRange = ColorRanges.Blue,
                loveFluff = new[] { "moonlight", "ice" },
                hateTheme = null,
                hateColorRange = ColorRanges.Yellow,
                hateFluff = new[] { "sunshine", "summer" },
            },

            // sportsy
            new()
            {
                customerName = "Katie",
                loveTheme = BagTheme.Sportsy,
                loveColorRange = ColorRanges.Pink,
                loveFluff = new[] { "trophies", "energy drinks" },
                hateTheme = BagTheme.Goth,
                hateColorRange = ColorRanges.Black,
                hateFluff = new[] { "skulls", "darkness" },
            },
            new()
            {
                customerName = "Sportina",
                loveTheme = BagTheme.Sportsy,
                loveColorRange = ColorRanges.Blue,
                loveFluff = new[] { "mornings", "sprinting" },
                hateTheme = BagTheme.Goth,
                hateColorRange = ColorRanges.Orange,
                hateFluff = new[] { "cigarettes", "slow walks" },
            },
            new()
            {
                customerName = "Rona Marathona",
                loveTheme = BagTheme.Sportsy,
                loveColorRange = ColorRanges.Orange,
                loveFluff = new[] { "running", "protein shakes" },
                hateTheme = BagTheme.Goth,
                hateColorRange = ColorRanges.Black,
                hateFluff = new[] { "elevators" },
            },

            // Y2K
            new()
            {
                customerName = "Britney B.",
                loveTheme = BagTheme.Y2K,
                loveColorRange = ColorRanges.Cyan,
                loveFluff = new[] { "flip phones" },
                hateTheme = BagTheme.Goth,
                hateColorRange = ColorRanges.Black,
                hateFluff = new[] { "metal", "darkness" },
            },
            new()
            {
                customerName = "Abril Lebin",
                loveTheme = BagTheme.Y2K,
                loveColorRange = ColorRanges.Pink,
                loveFluff = new[] { "low rise" },
                hateTheme = null,
                hateColorRange = ColorRanges.Green,
                hateFluff = new[] { "cargo pants" },
            },
            new()
            {
                customerName = "Veyonse",
                loveTheme = BagTheme.Y2K,
                loveColorRange = ColorRanges.Purple,
                loveFluff = new[] { "cloths", "jacuzzi"},
                hateTheme = BagTheme.Sportsy,
                hateColorRange = ColorRanges.White,
                hateFluff = new[] { "cheap shampoo", "minimalism" },
            },
        };

        // shuffle the list
        orderRequests = orderRequests.OrderBy(_ => Random.Range(0, int.MaxValue)).ToList();
        TotalMoney = 0;
        if (!GameOver)
        {
            StartCoroutine(SubmitHandler());
            moneyText.text = $"Money {TotalMoney}$";
            Submit.onClick.AddListener(() => StartCoroutine(SubmitHandler()));
        }
    }
    IEnumerator GetNewOrder()
    {
        if (GameOver) { yield break; }

        if (orderRequestIndex >= orderRequests.Count)
        {
            GameOver = true;
            Score.Instance.BagCount = orderRequests.Count;
            Score.Instance.TotalMoney = TotalMoney;
            MainMenu.GoToCompleted();
            yield break;
        }

        currentOrderRequest = orderRequests[orderRequestIndex];
        orderRequestIndex++;
        yield return SetNoteText();
    }

    void SetButtonsInteract(bool value)
    {
        Submit.interactable = value;
        Next.interactable = value;
        Prev.interactable = value;
    }
    IEnumerator SubmitHandler()
    {
        SetButtonsInteract(false);
        AudioManager.Instance.PlaySubmit();
        confettiParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        if (!isFirstOrder) confettiParticles.Play();
        BagManager.Instance.colorPicker.isActive = false;
        yield return FadeOut(instant: isFirstOrder);
        // calculate money, load next bag
        if (!isFirstOrder)
        {
            CalculateOrderValue();
        }
        BagManager.Instance.LoadNewBagFadedOut();
        yield return GetNewOrder();
        BagManager.Instance.DeleteAllTrinkets(); // clear for new bag
        BagManager.Instance.colorPicker.isActive = true;
        yield return FadeIn();
        AudioManager.Instance.PlayBubble();
        SetButtonsInteract(true);
        yield return null;
    }

    void CalculateOrderValue()
    {
        int _money = 0;
        var bag = BagManager.Instance.CurrentBag;
        var order = currentOrderRequest;
        Debug.Log($"Order: {order.customerName} | loveTheme: {order.loveTheme} | hateTheme: {order.hateTheme}");
        Debug.Log($"loveColorRange: {order.loveColorRange?.ToHex() ?? "null"} | hateColorRange: {order.hateColorRange?.ToHex() ?? "null"}");

        // Bag theme 
        if (order.loveTheme.HasValue && bag.theme == order.loveTheme.Value)
            _money += Random.Range(20, 30);

        if (order.hateTheme.HasValue && bag.theme == order.hateTheme.Value)
            _money -= Random.Range(8, 15);

        // Bag color
        Color bagColor = BagManager.Instance.GetLayer0Color();
        Color.RGBToHSV(bagColor, out float bagHue, out float bagSat, out float bagVal);
        Debug.Log($"Bag theme: {bag.theme} | Bag HSV: H={bagHue:F2} S={bagSat:F2} V={bagVal:F2}");
        Debug.Log($"Black range match: {IsInColorRange(bagHue, bagSat, bagVal, ColorRanges.Black)}");


        if (order.loveColorRange.HasValue && IsInColorRange(bagHue, bagSat, bagVal, order.loveColorRange.Value))
            _money += Random.Range(15, 25);

        if (order.hateColorRange.HasValue && IsInColorRange(bagHue, bagSat, bagVal, order.hateColorRange.Value))
            _money -= Random.Range(5, 15);

        // Trinkets
        int maxScoredTrinkets = 5;
        int trinketMoney = 0;

        foreach (var trinket in BagManager.Instance.GetTrinkets().Take(maxScoredTrinkets))
        {
            // Trinket theme
            if (order.loveTheme.HasValue && trinket.theme == order.loveTheme.Value)
                trinketMoney += Random.Range(10, 20);

            else if (order.hateTheme.HasValue && trinket.theme == order.hateTheme.Value)
                trinketMoney -= Random.Range(3, 9);

            // Trinket color
            Color.RGBToHSV(trinket.GetColor(), out float h, out float s, out float v);

            if (order.loveColorRange.HasValue && IsInColorRange(h, s, v, order.loveColorRange.Value))
                trinketMoney += Random.Range(5, 15);

            else if (order.hateColorRange.HasValue && IsInColorRange(h, s, v, order.hateColorRange.Value))
                trinketMoney -= Random.Range(3, 9);
        }

        _money += Mathf.Clamp(trinketMoney, -50, 50);
        _money = Mathf.Max(0, _money); // floor at 0

        if (_money > 80)
            AudioManager.Instance.PlayPleased();
        else if (_money > 40)
            AudioManager.Instance.PlayMedium();
        else
            AudioManager.Instance.PlayDislike();

        Invoke(nameof(PlayMoneySFX), 0.8f);
        TotalMoney += _money;
        moneyText.text = $"Money {TotalMoney}$";
    }
    private bool IsInColorRange(float hue, float sat, float val, ColorRange range)
    {
        return hue >= range.hueRange.x && hue <= range.hueRange.y &&
               sat >= range.satRange.x && sat <= range.satRange.y &&
               val >= range.valRange.x && val <= range.valRange.y;
    }
    void PlayMoneySFX() => AudioManager.Instance.PlayMoney();
    void Start()
    {
        Init();
        foreach (var (name, range) in new (string, ColorRange)[]
{
    ("Red",    ColorRanges.Red),
    ("Orange", ColorRanges.Orange),
    ("Yellow", ColorRanges.Yellow),
    ("Green",  ColorRanges.Green),
    ("Cyan",   ColorRanges.Cyan),
    ("Blue",   ColorRanges.Blue),
    ("Purple", ColorRanges.Purple),
    ("Pink",   ColorRanges.Pink),
    ("Black",  ColorRanges.Black),
    ("White",  ColorRanges.White),
    ("Grey",   ColorRanges.Grey),
})
        {
            Debug.Log($"{name}: {range.ToHex()}");
        }
    }

    IEnumerator SetNoteText()
    {
        if (isFirstOrder)
        {
            NoteCustomer.text = "";
            isFirstOrder = false;
            return TypeAllRoutine();
        }
        else
        {
            return StrikeThenTypeAll();
        }
    }

    IEnumerator TypeAllRoutine()
    {
        AudioManager.Instance.PlayScribble();
        yield return TypeRoutine(NoteCustomer, currentOrderRequest.GenerateOrderText());
        AudioManager.Instance.StopScribble();
    }

    IEnumerator StrikeAllRoutine()
    {
        yield return StrikeRoutine(NoteCustomer);
    }
    IEnumerator TypeRoutine(TextMeshProUGUI textComponent, string fullText)
    {
        textComponent.text = fullText; // set full text first
        textComponent.maxVisibleCharacters = 0;

        for (int i = 0; i <= fullText.Length; i++)
        {
            var delay = Random.Range(0.02f, 0.08f);
            delay = 0.02f;
            textComponent.maxVisibleCharacters = i;
            yield return new WaitForSeconds(delay);
        }
        yield return null;
    }

    IEnumerator StrikeRoutine(TMP_Text textComponent)
    {
        string original = textComponent.text;

        for (int i = 1; i <= original.Length; i++)
        {
            string struck = original.Substring(0, i);
            string rest = original.Substring(i);
            textComponent.text = $"<s>{struck}</s>{rest}";
            yield return new WaitForSeconds(0.03f);
        }
    }
    IEnumerator StrikeThenTypeAll()
    {
        yield return StrikeAllRoutine();
        yield return FadeAllRoutine();
        yield return TypeAllRoutine();
    }

    IEnumerator FadeAllRoutine()
    {
        yield return FadeRoutine(NoteCustomer);
    }

    IEnumerator FadeRoutine(TMP_Text textComponent)
    {
        float duration = 0.4f;
        float elapsed = 0f;
        Color original = textComponent.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            textComponent.color = new Color(original.r, original.g, original.b, alpha);
            yield return null;
        }

        textComponent.text = "";
        textComponent.color = original; // reset alpha for typing
    }

    IEnumerator FadeOut(float duration = 0.5f, bool instant = false)
    {
        var images = LayersGroup.GetComponentsInChildren<Image>();
        if (instant)
        {
            foreach (var img in images)
                img.color = new Color(img.color.r, img.color.g, img.color.b, 0f);
            yield break;
        }
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            foreach (var img in images)
                img.color = new Color(img.color.r, img.color.g, img.color.b, alpha);
            yield return null;
        }
    }

    IEnumerator FadeIn(float duration = 0.5f)
    {
        var images = LayersGroup.GetComponentsInChildren<Image>();
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            foreach (var img in images)
                img.color = new Color(img.color.r, img.color.g, img.color.b, alpha);
            yield return null;
        }
    }
}