using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : SingletonPerScene<GameManager>
{

    [SerializeField] Button Submit;
    [SerializeField] ConfirmDialog Dialog;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] TextMeshProUGUI NoteCustomer;
    [SerializeField] TextMeshProUGUI NoteTheme;
    [SerializeField] TextMeshProUGUI NoteLike;
    [SerializeField] TextMeshProUGUI NoteHate;
    float timeRemaining = 180f; // seconds
    int TotalMoney;
    private bool timerRunning;
    List<OrderRequest> orderRequests;
    OrderRequest currentOrderRequest;
    int orderRequestIndex = 0;

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
        timerRunning = true;
        GetNewOrder();
        moneyText.text = $"Money {TotalMoney}$";
        Submit.onClick.AddListener(SubmitHandler);
    }
    void GetNewOrder()
    {
        if (orderRequestIndex >= orderRequests.Count)
        {
            ConfirmDialog.Show("Completed!", "Back to main menu.", onConfirm: MainMenu.GoToMainMenu, backToMenu: true);
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

    void DisplayTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void OnTimerEnd()
    {
        // load end level screen
        Debug.Log("time is up");
    }
    void Update()
    {
        if (timerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                timeRemaining = 0;
                timerRunning = false;
                DisplayTime(0);
                OnTimerEnd();
            }
        }
    }

    void SetNoteText()
    {
        NoteCustomer.text = $"Customer: {currentOrderRequest.customerName}";
        NoteTheme.text = $"Theme: {currentOrderRequest.theme}";
        NoteLike.text = $"Loves: {currentOrderRequest.LoveText}";
        NoteHate.text = $"Hates: {currentOrderRequest.HateText}";
    }
}