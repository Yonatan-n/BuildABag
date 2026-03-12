using System.Collections;
using System.Collections.Generic;
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
            new() {
                theme = BagTheme.Goth,
                customerName = "Gotty",
                LoveText = "Metal",
                HateText="Bright Colors",
                lovedTrinkets = new List<TrinketType> { TrinketType.Metal, TrinketType.Keychain },
                hatedTrinkets = new List<TrinketType> { TrinketType.Sticker },
            },
            new() {
                theme = BagTheme.Girly,
                customerName = "Pinka",
                LoveText = "Pink",
                HateText="Skulls, Darkness",
                lovedTrinkets = new List<TrinketType> { TrinketType.Sticker, TrinketType.Cloth},
                hatedTrinkets = new List<TrinketType> { TrinketType.Metal, TrinketType.Keychain },
            },
          new() {
                theme = BagTheme.Y2K,
                customerName = "Britney B.",
                LoveText = "Bright Colors, random items",
                HateText="Metal",
                lovedTrinkets = new List<TrinketType> { TrinketType.Sticker, TrinketType.Keychain, TrinketType.Cloth },
                hatedTrinkets = new List<TrinketType> { TrinketType.Metal },
            },
        };
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
            TotalMoney += 100;
            // play pleaser
        }
        else
        {
            TotalMoney += 50;
            // play meduim
        }
        AudioManager.Instance.PlayMoney();
        moneyText.text = $"Money {TotalMoney}$";
        // TODO: implement
    }

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