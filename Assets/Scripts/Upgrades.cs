using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum UpgradeType
{
    MoneyPerClick,
    MoneyPerSecond,
    ClickMultiplier,
    MultiplierOverTime,
    CritChance,
    BonusEveryFiveClicks,
    AllIncomeMultiplier
}

public class Upgrade : MonoBehaviour
{
    [Header("Ustawienia ulepszenia")]
    [SerializeField] private string upgradeName = "Upgrade";
    [TextArea(2, 4)]
    [SerializeField] private string description = "Opis ulepszenia...";
    [SerializeField] public double cost = 10;
    [SerializeField] private double upgradeAmount = 1;
    [SerializeField] private UpgradeType type;
    [SerializeField] private Sprite upgradeSprite;
    public Sprite UpgradeSprite => upgradeSprite;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button upgradeButton;

    [Header("Kolory przycisków")]
    [SerializeField] private Color affordableColor = new Color(0.66f, 0.90f, 0.63f); 
    [SerializeField] private Color unaffordableColor = new Color(0.96f, 0.64f, 0.64f); 

    public bool IsUnlocked { get; private set; } = false;

    private void Start()
    {
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(BuyUpgrade);

        UpdateUI();

        if (PlayerStats.Instance != null)
            PlayerStats.Instance.OnMoneyChanged += UpdateButtonColor;
    }

    private void OnDestroy()
    {
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.OnMoneyChanged -= UpdateButtonColor;
    }

    private void BuyUpgrade()
    {
        var player = PlayerStats.Instance;

        if (player.Money < cost)
            return;

        player.Money -= cost;

        switch (type)
        {
            case UpgradeType.MoneyPerClick:
                player.MoneyPerClick += upgradeAmount;
                AudioManager.Instance.Play("miku");
                break;
            case UpgradeType.ClickMultiplier:
                player.MultiplierClicks += (int)upgradeAmount;
                AudioManager.Instance.Play("jinx");
                break;
            case UpgradeType.MoneyPerSecond:
                player.MoneyPerSecond += upgradeAmount;
                AudioManager.Instance.Play("tao");
                break;
            case UpgradeType.MultiplierOverTime:
                player.MultiplierOverTime += upgradeAmount;
                AudioManager.Instance.Play("columbina");
                break;
            case UpgradeType.CritChance:
                AudioManager.Instance.Play("kuromi");
                if (player.CritChance < 100)
                    player.CritChance += upgradeAmount;
                else
                    player.CritMultiplier += 0.5;
                break;
            case UpgradeType.BonusEveryFiveClicks:
                AudioManager.Instance.Play("pusheen");
                if (player.ClickBonusAmount == 0)
                {
                    player.ClickBonusAmount = 50;
                    player.ClicksSinceLastBonus = 0;
                }
                else
                {
                    player.ClickBonusAmount *= 2;
                }
                break;
            case UpgradeType.AllIncomeMultiplier:
                player.AllIncomeMultiplier += upgradeAmount;
                AudioManager.Instance.Play("milk");
                break;
        }

        cost *= 2;
        UpdateUI();
        IsUnlocked = true;
        player.NotifyMoneyChanged(); 
    }

    private void UpdateUI()
    {
        if (nameText != null)
            nameText.text = upgradeName;

        if (descriptionText != null)
            descriptionText.text = description;

        if (costText != null)
            costText.text = $"${cost:F0}";

        UpdateButtonColor();
    }

    private void UpdateButtonColor()
    {
        if (upgradeButton == null || PlayerStats.Instance == null)
            return;

        var player = PlayerStats.Instance;
        var colors = upgradeButton.colors;

        if (player.Money >= cost)
        {
            colors.normalColor = affordableColor;
            colors.highlightedColor = affordableColor;
        }
        else
        {
            colors.normalColor = unaffordableColor;
            colors.highlightedColor = unaffordableColor;
        }

        upgradeButton.colors = colors;
    }
}
