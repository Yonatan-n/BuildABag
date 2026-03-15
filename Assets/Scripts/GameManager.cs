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
    [SerializeField] TextMeshProUGUI NoteTheme;
    [SerializeField] TextMeshProUGUI NoteLike;
    [SerializeField] TextMeshProUGUI NoteHate;
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
                loveColorRange  = ColorRanges.Black,
                loveFluff = new[] { "death" },
                hateTheme = null,
                hateColorRange = ColorRanges.Yellow,
                hateFluff = new[] { "mornings" },
            },
            // new()
            // {
            //     theme = BagTheme.Goth,
            //     customerName = "Lilith",
            //     LoveText = "Darkness",
            //     HateText = "Logos and Icons",
            // },
            // new()
            // {
            //     theme = BagTheme.Goth,
            //     customerName = "Bianca",
            //     LoveText = "Skulls",
            //     HateText = "Pink and Purple",
            // },
            // new()
            // {
            //     theme = BagTheme.Goth,
            //     customerName = "Elsa",
            //     LoveText = "Gray and Blue",
            //     HateText = "Yellow and Red",
            // },
            // // sportsy
            // new()
            // {
            //     theme = BagTheme.Sportsy,
            //     customerName = "Katie",
            //     LoveText = "Pink and Trinkets",
            //     HateText = "Skulls, Darkness",
            // },
            // new()
            // {
            //     theme = BagTheme.Sportsy,
            //     customerName = "Sportina",
            //     LoveText = "Blue and Keychains",
            //     HateText = "Pink and Cigarettes",
            // },
            // new()
            // {
            //     theme = BagTheme.Sportsy,
            //     customerName = "Rona Marathona",
            //     LoveText = "Running. FAST.",
            //     HateText = "Dark colors",
            // },
            // // Y2K
            // new()
            // {
            //     theme = BagTheme.Y2K,
            //     customerName = "Britney B.",
            //     LoveText = "Bright Colors, random items",
            //     HateText = "Metal",
            // },
            // new()
            // {
            //     theme = BagTheme.Y2K,
            //     customerName = "Abril Lebin",
            //     LoveText = "Pink and Black",
            //     HateText = "Green and Yellow",
            // },
            // new()
            // {
            //     theme = BagTheme.Y2K,
            //     customerName = "Veyonse",
            //     LoveText = "Purple",
            //     HateText = "White and Gray",
            // },
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
            NoteTheme.text = "";
            NoteLike.text = "";
            NoteHate.text = "";
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
        // yield return TypeRoutine(NoteCustomer, $"Customer: {currentOrderRequest.customerName}");
        // yield return TypeRoutine(NoteTheme, $"Theme: {currentOrderRequest.theme}");
        // yield return TypeRoutine(NoteLike, $"Loves: {currentOrderRequest.LoveText}");
        // yield return TypeRoutine(NoteHate, $"Hates: {currentOrderRequest.HateText}");
        AudioManager.Instance.StopScribble();
    }

    IEnumerator StrikeAllRoutine()
    {
        yield return StrikeRoutine(NoteCustomer);
        yield return StrikeRoutine(NoteTheme);
        yield return StrikeRoutine(NoteLike);
        yield return StrikeRoutine(NoteHate);
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
        yield return FadeRoutine(NoteTheme);
        yield return FadeRoutine(NoteLike);
        yield return FadeRoutine(NoteHate);
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