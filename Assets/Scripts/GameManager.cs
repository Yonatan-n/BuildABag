using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
            new() {
                theme = BagTheme.Goth,
                customerName = "Astrid",
                LoveText = "Metal",
                HateText="Green and Yellow",
            },
            new() {
                theme = BagTheme.Goth,
                customerName = "Lilith",
                LoveText = "Darkness",
                HateText="Logos and Icons",
            },
            new() {
                theme = BagTheme.Goth,
                customerName = "Bianca",
                LoveText = "Skulls",
                HateText="Pink and Purple",
            },
            new() {
                theme = BagTheme.Goth,
                customerName = "Elsa",
                LoveText = "Gray and Blue",
                HateText="Yellow and Red",
            },
            // sportsy
            new() {
                theme = BagTheme.Sportsy,
                customerName = "Katie",
                LoveText = "Pink and Trinkets",
                HateText="Skulls, Darkness",
            },
            new() {
                theme = BagTheme.Sportsy,
                customerName = "Sportina",
                LoveText = "Blue and Keychains",
                HateText="Pink and Cigarettes",
            },
            new() {
                theme = BagTheme.Sportsy,
                customerName = "Rona Marathona",
                LoveText = "Running. FAST.",
                HateText="Dark colors",
            },
            // Y2K
            new() {
                theme = BagTheme.Y2K,
                customerName = "Britney B.",
                LoveText = "Bright Colors, random items",
                HateText="Metal",
            },
            new() {
                theme = BagTheme.Y2K,
                customerName = "Abril Lebin",
                LoveText = "Pink and Black",
                HateText="Green and Yellow",
            },
            new() {
                theme = BagTheme.Y2K,
                customerName = "Veyonse",
                LoveText = "Purple",
                HateText="White and Gray",
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
        if (GameOver) { yield return null; }

        if (orderRequestIndex >= orderRequests.Count)
        {
            GameOver = true;
            Score.Instance.BagCount = orderRequests.Count;
            Score.Instance.TotalMoney = TotalMoney;
            MainMenu.GoToCompleted();
            yield return null;
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
        yield return FadeOut(instant: isFirstOrder);
        BagManager.Instance.LoadNewBagFadedOut();
        // calculate money, load next bag
        if (!isFirstOrder)
        {
            CalculateOrderValue();
        }
        yield return GetNewOrder();
        yield return FadeIn();
        AudioManager.Instance.PlayBubble();
        SetButtonsInteract(true);
        yield return null;
    }

    void CalculateOrderValue()
    {
        var varient = BagManager.Instance.CurrentBag.variants[0]; // always the first for now
        if (varient.theme == currentOrderRequest.theme)
        {
            AudioManager.Instance.PlayPleased();
            TotalMoney += 100;
        }
        else
        {
            AudioManager.Instance.PlayMedium();
            TotalMoney += 50;
        }
        Invoke(nameof(PlayMoneySFX), 0.8f);
        moneyText.text = $"Money {TotalMoney}$";
        // TODO: implement
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
        yield return TypeRoutine(NoteCustomer, $"Customer: {currentOrderRequest.customerName}");
        yield return TypeRoutine(NoteTheme, $"Theme: {currentOrderRequest.theme}");
        yield return TypeRoutine(NoteLike, $"Loves: {currentOrderRequest.LoveText}");
        yield return TypeRoutine(NoteHate, $"Hates: {currentOrderRequest.HateText}");
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