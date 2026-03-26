using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

struct ScoringItem
{
    public BagTheme Theme;
    public Color Color;
}

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
            new()
            {
                customerName = "Veyonse",
                loveTheme = BagTheme.Y2K,
                loveColorRanges = new List<ColorRange> { ColorRanges.Purple },
                loveFluff = new[] { "expensive wine", "jacuzzi" },
                hateTheme = BagTheme.Sportsy,
                hateColorRanges = new List<ColorRange> { ColorRanges.White },
                hateFluff = new[] { "cheap shampoo", "minimalism" },
            },
            // goth 
            new()
            {
                customerName = "Astrid",
                loveTheme = BagTheme.Goth,
                loveColorRanges = new List<ColorRange> { ColorRanges.Purple },
                loveFluff = new[] { "death", "ravens" },
                hateTheme = null,
                hateColorRanges = new List<ColorRange> { ColorRanges.Yellow, ColorRanges.Orange },
                hateFluff = new[] { "mornings" },
            },
            new()
            {
                customerName = "Lilith",
                loveTheme = BagTheme.Goth,
                loveColorRanges = new List<ColorRange> { ColorRanges.Black },
                loveFluff = new[] { "darkness", "candles" },
                hateTheme = BagTheme.Girly,
                hateColorRanges = new List<ColorRange> { ColorRanges.Green, ColorRanges.Red },
                hateFluff = new[] { "apples", "sheep" },
            },
            // y2k
            new()
            {
                customerName = "Britney B.",
                loveTheme = BagTheme.Y2K,
                loveColorRanges = new List<ColorRange> { ColorRanges.Cyan, ColorRanges.Pink },
                loveFluff = new[] { "flip phones" },
                hateTheme = null,
                hateColorRanges = new List<ColorRange> { ColorRanges.Grey },
                hateFluff = new[] { "paparazzi", "mushrooms" },
            },
            new()
            {
                customerName = "Abril Lebin",
                loveTheme = BagTheme.Y2K,
                loveColorRanges = new List<ColorRange> { ColorRanges.Pink, ColorRanges.Blue },
                loveFluff = new[] { "low rise" },
                hateTheme = null,
                hateColorRanges = new List<ColorRange> { ColorRanges.Green },
                hateFluff = new[] { "cargo pants" },
            },
            // sportsy
            new()
            {
                customerName = "Rona Marathona",
                loveTheme = BagTheme.Sportsy,
                loveColorRanges = new List<ColorRange> { ColorRanges.Orange, ColorRanges.Cyan },
                loveFluff = new[] { "running", "protein shakes" },
                hateTheme = null,
                hateColorRanges = new List<ColorRange> { ColorRanges.Purple, ColorRanges.Pink },
                hateFluff = new[] { "elevators", "sittings" },
            },
            new()
            {
                customerName = "Katie",
                loveTheme = BagTheme.Sportsy,
                loveColorRanges = new List<ColorRange> { ColorRanges.Pink, ColorRanges.Blue },
                loveFluff = new[] { "wining", "energy drinks" },
                hateTheme = BagTheme.Goth,
                hateColorRanges = new List<ColorRange> { ColorRanges.Black, ColorRanges.White },
                hateFluff = new[] { "losing" },
            },
            // goth
            new()
            {
                customerName = "Bianca",
                loveTheme = BagTheme.Goth,
                loveColorRanges = new List<ColorRange> { ColorRanges.Grey, },
                loveFluff = new[] { "skulls", "bones" },
                hateTheme = null,
                hateColorRanges = new List<ColorRange> { ColorRanges.Pink, ColorRanges.Yellow },
                hateFluff = new[] { "glitter", "rainbows" },
            },
            new()
            {
                customerName = "Elsa",
                loveTheme = BagTheme.Goth,
                loveColorRanges = new List<ColorRange> { ColorRanges.Blue,},
                loveFluff = new[] { "moonlight", "ice" },
                hateTheme = null,
                hateColorRanges = new List<ColorRange> { ColorRanges.Red},
                hateFluff = new[] { "sunshine", "summer" },
            },
            // sportsy
            new()
            {
                customerName = "Sportina",
                loveTheme = BagTheme.Sportsy,
                loveColorRanges = new List<ColorRange> { ColorRanges.Blue, ColorRanges.Red },
                loveFluff = new[] { "mornings", "sprinting" },
                hateTheme = BagTheme.Goth,
                hateColorRanges = new List<ColorRange> { ColorRanges.Orange, ColorRanges.Green },
                hateFluff = new[] { "cigarettes", "slow walks" },
            },
        };
        // shuffle the list
        // orderRequests = orderRequests.OrderBy(_ => Random.Range(0, int.MaxValue)).ToList();
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
        yield return SetNoteText();
    }

    void SetButtonsInteract(bool value)
    {
        Submit.interactable = value;
        Next.interactable = value;
        Prev.interactable = value;
    }
    bool IsThemeChanging()
    {
        int current = isFirstOrder ? orderRequestIndex : orderRequestIndex - 1;
        int next = current + 1;
        return next < orderRequests.Count &&
               orderRequests[current].loveTheme != orderRequests[next].loveTheme;
    }

    IEnumerator SubmitHandler()
    {
        SetButtonsInteract(false);
        AudioManager.Instance.PlaySubmit();
        confettiParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        if (!isFirstOrder) confettiParticles.Play();
        BagManager.Instance.colorPicker.isActive = false;
        yield return FadeOut(instant: isFirstOrder);

        if (!isFirstOrder && IsThemeChanging()) // don't change on first order, check for game over
            AudioManager.Instance.TransitionToTrackByTheme((BagTheme)orderRequests[orderRequestIndex].loveTheme);
        if (!isFirstOrder)
            CalculateOrderValue();

        BagManager.Instance.LoadNewBagFadedOut();
        yield return GetNewOrder();
        orderRequestIndex++;
        BagManager.Instance.DeleteAllTrinkets();
        BagManager.Instance.colorPicker.isActive = true;

        yield return FadeIn();
        AudioManager.Instance.PlayBubble();
        SetButtonsInteract(true);
        yield return null;
    }
    private int ScoreItem(ScoringItem item, OrderRequest order,
        (int min, int max) loveTheme,
        (int min, int max) hateTheme,
        (int min, int max) loveColor,
        (int min, int max) hateColor)
    {
        int score = 0;

        if (order.loveTheme.HasValue && item.Theme == order.loveTheme.Value)
            score += Random.Range(loveTheme.min, loveTheme.max);

        if (order.hateTheme.HasValue && item.Theme == order.hateTheme.Value)
            score -= Random.Range(hateTheme.min, hateTheme.max);

        Color.RGBToHSV(item.Color, out float h, out float s, out float v);

        if (IsInAnyColorRange(h, s, v, order.loveColorRanges))
            score += Random.Range(loveColor.min, loveColor.max);

        if (IsInAnyColorRange(h, s, v, order.hateColorRanges))
            score -= Random.Range(hateColor.min, hateColor.max);

        return score;
    }

    void CalculateOrderValue()
    {
        int _money = 0;
        var order = currentOrderRequest;

        _money += ScoreItem(
            new ScoringItem { Theme = BagManager.Instance.CurrentBag.theme, Color = BagManager.Instance.GetLayer0Color() },
            order,
            loveTheme: (20, 30),
            hateTheme: (8, 15),
            loveColor: (15, 25),
            hateColor: (15, 25));


        int trinketMoney = 0;
        foreach (var trinket in BagManager.Instance.GetTrinkets().Take(5))
        {
            trinketMoney += ScoreItem(
                new ScoringItem { Theme = trinket.theme, Color = trinket.GetColor() },
                order,
                loveTheme: (10, 20),
                hateTheme: (3, 9),
                loveColor: (5, 15),
                hateColor: (3, 9));
        }

        _money += Mathf.Clamp(trinketMoney, -50, 50);
        _money = Mathf.Max(1, _money);

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
    private bool IsInAnyColorRange(float h, float s, float v, List<ColorRange> ranges)
    {
        if (ranges == null) return false;
        return ranges.Any(range => IsInColorRange(h, s, v, range));
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
                img.color = new Color(img.color.r, img.color.g, img.color.b, img.sprite == null ? 0f : alpha);
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
                img.color = new Color(img.color.r, img.color.g, img.color.b, img.sprite == null ? 0f : alpha);
            yield return null;
        }
    }
}