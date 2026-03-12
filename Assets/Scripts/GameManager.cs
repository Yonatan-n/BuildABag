using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : SingletonPerScene<GameManager>
{

    [SerializeField] Button Submit;
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] TextMeshProUGUI NoteCustomer;
    [SerializeField] TextMeshProUGUI NoteTheme;
    [SerializeField] TextMeshProUGUI NoteLike;
    [SerializeField] TextMeshProUGUI NoteHate;
    [SerializeField] ParticleSystem confettiParticles;

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
        GetNewOrder();
        if (!GameOver)
        {
            moneyText.text = $"Money {TotalMoney}$";
            Submit.onClick.AddListener(SubmitHandler);
        }
    }
    void GetNewOrder()
    {
        if (GameOver) { return; }

        if (orderRequestIndex >= orderRequests.Count)
        {
            GameOver = true;
            MainMenu.GoToCompleted();
            return;
        }

        currentOrderRequest = orderRequests[orderRequestIndex];
        orderRequestIndex++;
        SetNoteText();
    }
    void SubmitHandler()
    {
        // calculate money, load next bag
        confettiParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        confettiParticles.Play();
        // TODO: fade bag to black
        CalculateOrderValue();
        GetNewOrder();
        AudioManager.Instance.PlaySubmit();
        Debug.Log("submit");
    }

    void CalculateOrderValue()
    {
        TotalMoney += 100;
        moneyText.text = $"Money {TotalMoney}$";

        // TODO: implement
    }

    void Start()
    {
        Init();
    }

    void SetNoteText()
    {
        if (isFirstOrder)
        {
            NoteCustomer.text = "";
            NoteTheme.text = "";
            NoteLike.text = "";
            NoteHate.text = "";
            StartCoroutine(TypeAllRoutine());
            isFirstOrder = false;
        }
        else
        {
            StartCoroutine(StrikeThenTypeAll());
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
}