using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private Button fishlandButton;
    [SerializeField] private Button jungleButton;
    [SerializeField] private Button marketButton;
    [SerializeField] private TMP_Text headerText;

    [Header("Prices")]
    [SerializeField] private int fishlandPrice = 10;
    [SerializeField] private int junglePrice = 20;
    [SerializeField] private int marketPrice = 30;

    [Header("Backgrounds")]
    [SerializeField] private Sprite defaultBG;
    [SerializeField] private Sprite fishlandBG;
    [SerializeField] private Sprite jungleBG;
    [SerializeField] private Sprite marketBG;
    [SerializeField] private Image gameBackground;

    [Header("Environment Icons")]
    [SerializeField] public GameObject numbersParent;
    [SerializeField] public Sprite fishIcon;
    [SerializeField] public Sprite bananaIcon;
    [SerializeField] public Sprite numberIcon;

    private const string CoinsKey = "Coins";
    private const string SelectedEnvKey = "SelectedEnvironment";
    public string CurrentSelectedEnvironment = "Fishland";
    public Sprite currentSelectedIcon;
    public int totalCOins;

    void Start()
    {
        totalCOins = PlayerPrefs.GetInt(CoinsKey, 0);
        InitializeButtons();
        UpdateCoinDisplay();
        CheckUnlockStatus();
        LoadSelectedEnvironment();
        headerText.text = CurrentSelectedEnvironment;
    }

    private void InitializeButtons()
    {
       
        if (fishlandButton != null)
        {
            fishlandButton.onClick.RemoveAllListeners();
            fishlandButton.onClick.AddListener(() => OnEnvironmentClick("Fishland", fishlandPrice, "FishlandUnlocked", fishlandBG, fishIcon));
        }

        if (jungleButton != null)
        {
            jungleButton.onClick.RemoveAllListeners();
            jungleButton.onClick.AddListener(() => OnEnvironmentClick("Jungle", junglePrice, "JungleUnlocked", jungleBG, bananaIcon));
        }

        if (marketButton != null)
        {
            marketButton.onClick.RemoveAllListeners();
            marketButton.onClick.AddListener(() => OnEnvironmentClick("Market", marketPrice, "MarketUnlocked", marketBG, numberIcon));
        }
    }

    private void UpdateCoinDisplay()
    {
        coinsText.text = totalCOins.ToString();
    }

    private void CheckUnlockStatus()
    {
        SetButtonText(fishlandButton, "FishlandUnlocked");
        SetButtonText(jungleButton, "JungleUnlocked");
        SetButtonText(marketButton, "MarketUnlocked");
    }

    private void SetButtonText(Button btn, string unlockKey)
    {
        if (btn != null && PlayerPrefs.GetInt(unlockKey, 0) == 1)
        {
            TMP_Text btnText = btn.GetComponentInChildren<TMP_Text>();
            if (btnText != null)
                btnText.text = "Purchased";
        }
    }

    private void OnEnvironmentClick(string name, int price, string key, Sprite bg, Sprite iconSet)
    {
        int coins = PlayerPrefs.GetInt(CoinsKey, 0);
        bool isUnlocked = PlayerPrefs.GetInt(key, 0) == 1;

        if (isUnlocked)
        {
            SelectEnvironment(name, bg, iconSet);
        }
        else if (coins >= price)
        {
            PlayerPrefs.SetInt(CoinsKey, coins - price);
            PlayerPrefs.SetInt(key, 1);
            PlayerPrefs.Save(); // Force save immediately

            SelectEnvironment(name, bg, iconSet);
            UpdateCoinDisplay();
            CheckUnlockStatus();
            GameManager.Instance.OnEnvironmentChange();
            Debug.Log("New Theme Unlocked: " + name);

        }
        else
        {
            Debug.LogWarning("Not enough coins!");
        }
    }

    private void SelectEnvironment(string name, Sprite bg, Sprite iconSet)
    {
        PlayerPrefs.SetString(SelectedEnvKey, name);
        PlayerPrefs.Save(); // Force save immediately

        CurrentSelectedEnvironment = name;

        if (gameBackground != null && bg != null)
            gameBackground.sprite = bg;

        currentSelectedIcon = iconSet;

        Debug.Log($"{name} selected!");
        headerText.text = name;
    }

    private void LoadSelectedEnvironment()
    {
        CurrentSelectedEnvironment = PlayerPrefs.GetString(SelectedEnvKey, "Fishland");

        switch (CurrentSelectedEnvironment)
        {
            case "Fishland":
                if (gameBackground != null && fishlandBG != null)
                    gameBackground.sprite = fishlandBG;
                currentSelectedIcon = fishIcon;
                break;
            case "ClassRoom":
                if (gameBackground != null && defaultBG != null)
                    gameBackground.sprite = defaultBG;
                currentSelectedIcon = null;
                break;
            case "Jungle":
                if (gameBackground != null && jungleBG != null)
                    gameBackground.sprite = jungleBG;
                currentSelectedIcon = bananaIcon;
                break;
            case "Market":
                if (gameBackground != null && marketBG != null)
                    gameBackground.sprite = marketBG;
                currentSelectedIcon = numberIcon;
                break;
        }
    }

    public void AddCoins(int amount)
    {
        totalCOins += amount;
        PlayerPrefs.SetInt(CoinsKey, totalCOins);
        PlayerPrefs.Save();
        UpdateCoinDisplay();
    }

    void OnDisable()
    {
        // Clean up button listeners
        if (fishlandButton != null)
            fishlandButton.onClick.RemoveAllListeners();
        if (jungleButton != null)
            jungleButton.onClick.RemoveAllListeners();
        if (marketButton != null)
            marketButton.onClick.RemoveAllListeners();

        // Save any pending PlayerPrefs
        PlayerPrefs.Save();
    }

    void OnDestroy()
    {
        OnDisable();
    }
}