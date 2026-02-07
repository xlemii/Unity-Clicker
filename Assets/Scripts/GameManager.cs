using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class GameManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI clickText;
    [SerializeField] private TextMeshProUGUI moneyPerClickText;
    [SerializeField] private TextMeshProUGUI moneyPerSecondText;
    [SerializeField] private TextMeshProUGUI multiplierClicksText;
    [SerializeField] private TextMeshProUGUI multiplierOverTimeText;
    [SerializeField] private TextMeshProUGUI critChanceText;
    [SerializeField] private TextMeshProUGUI critMultiplierText;
    [SerializeField] private TextMeshProUGUI allincomeText;

    [Header("UI Button")]
    [SerializeField] private Button clickButton;

    [Header("Floating Text")]
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private RectTransform floatingArea;

    [Header("Spadające ikony")]
    [SerializeField] private GameObject fallingIconPrefab;
    [SerializeField] private RectTransform iconSpawnArea;



    private void Start()
    {
        PlayerStats.Instance.OnMoneyChanged += UpdateUI;
        clickButton.onClick.AddListener(OnClick);
        PlayerStats.Instance.OnPassiveIncomeEarned += ShowPassiveFloatingText;
        UpdateUI();
    }

    private void ShowPassiveFloatingText(double income)
    {
        ShowFloatingText(income, false, false); 
    }

    private void ShowFloatingText(double amount, bool isCrit = false, bool isBonus = false)
    {
        if (floatingTextPrefab == null || floatingArea == null) return;

        GameObject obj = Instantiate(floatingTextPrefab, floatingArea);

        RectTransform rect = obj.GetComponent<RectTransform>();

        float padding = 20f; 
        float randomX = Random.Range(padding, floatingArea.rect.width - padding);
        float randomY = Random.Range(padding, floatingArea.rect.height - padding);

        rect.anchoredPosition = new Vector2(
            randomX - floatingArea.rect.width / 2f,
            randomY - floatingArea.rect.height / 2f
        );

        var textComp = obj.GetComponent<FloatingText>();
        if (textComp != null)
        {
            textComp.SetText($"+{amount:F0}$");

            if (isBonus)
                textComp.SetColor(Color.cyan);
            else if (isCrit)
                textComp.SetColor(Color.red);
            else
                textComp.SetColor(Color.green);
        }
    }


    private void OnClick()
    {
        var stats = PlayerStats.Instance;
        double earned = stats.MoneyPerClick * stats.MultiplierClicks;
        bool isCrit = Random.Range(0f, 100f) < stats.CritChance;
        if (isCrit)
            earned *= stats.CritMultiplier;
        earned *= stats.AllIncomeMultiplier;

        stats.Money += earned;
        stats.TotalClicks++;
        stats.NotifyMoneyChanged();
        ShowFloatingText(earned, isCrit);

        stats.ClicksSinceLastBonus++;
        if (stats.ClicksSinceLastBonus >= 5 && stats.ClickBonusAmount > 0)
        {
            double bonus = stats.ClickBonusAmount * stats.AllIncomeMultiplier;
            stats.Money += bonus;
            stats.ClicksSinceLastBonus = 0;
            ShowFloatingText(bonus, false, true);
        }
        AudioManager.Instance.Play("click");
        SpawnUpgradeIcons();
    }

    private void SpawnUpgradeIcons()
    {
        if (fallingIconPrefab == null || iconSpawnArea == null) return;

        var allUpgrades = FindObjectsByType<Upgrade>(FindObjectsSortMode.None);

        foreach (var upgrade in allUpgrades)
        {
            if (!upgrade.IsUnlocked || upgrade.UpgradeSprite == null)
                continue;

            int count = Mathf.Clamp((int)(upgrade.cost / 50), 1, 3);

            for (int i = 0; i < count; i++)
            {
                GameObject icon = Instantiate(fallingIconPrefab, iconSpawnArea);
                RectTransform rect = icon.GetComponent<RectTransform>();

                rect.anchoredPosition = new Vector2(
                    Random.Range(-iconSpawnArea.rect.width / 2f, iconSpawnArea.rect.width / 2f),
                    iconSpawnArea.rect.height / 2f + 100f
                );

                var falling = icon.GetComponent<FallingUpgradeIcon>();
                if (falling != null)
                    falling.SetSprite(upgrade.UpgradeSprite);
            }
        }
    }




    public void UpdateUI()
    {
        var stats = PlayerStats.Instance;

        if (moneyText != null)
            moneyText.text = $"MEOWCOINS: {stats.Money:F0}";

        if (clickText != null)
            clickText.text = $"TOTAL CLICKS: {stats.TotalClicks}";

        if (moneyPerClickText != null)
            moneyPerClickText.text = $"Money/Click: {stats.MoneyPerClick:F1}";

        if (moneyPerSecondText != null)
            moneyPerSecondText.text = $"Money/Sec: {stats.MoneyPerSecond:F1}";

        if (multiplierClicksText != null)
            multiplierClicksText.text = $"Click Multiplier: x{stats.MultiplierClicks:F1}";

        if (multiplierOverTimeText != null)
            multiplierOverTimeText.text = $"Time Multiplier: x{stats.MultiplierOverTime:F1}";
        
        if (critChanceText != null)
            critChanceText.text = $"Crit Chance: {PlayerStats.Instance.CritChance:F1}%";

        if (critMultiplierText != null)
            critMultiplierText.text = $"Crit Multiplier: {PlayerStats.Instance.CritMultiplier:F1}x";
        
        if (allincomeText != null)
            allincomeText.text = $"Bonus Income: {PlayerStats.Instance.AllIncomeMultiplier:F1}x";
    }
}
