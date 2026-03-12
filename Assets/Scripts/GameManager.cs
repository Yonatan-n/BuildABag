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
    int TotalMoney;
    List<OrderRequest> orderRequests;
    OrderRequest currentOrderRequest;
    int orderRequestIndex = 0;
    public bool GameOver = false;

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
        NoteCustomer.text = $"Customer: {currentOrderRequest.customerName}";
        NoteTheme.text = $"Theme: {currentOrderRequest.theme}";
        NoteLike.text = $"Loves: {currentOrderRequest.LoveText}";
        NoteHate.text = $"Hates: {currentOrderRequest.HateText}";
    }
}