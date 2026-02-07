using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    public double Money { get; set; } = 0;
    public double MoneyPerClick { get; set; } = 1; 
    public double MoneyPerSecond { get; set; } = 0;   
    public double MultiplierClicks { get; set; } = 1;
    public double MultiplierOverTime { get; set; } = 1; 
    public double CritChance { get; set; } = 0;
    public double CritMultiplier { get; set; } = 2.0;
    public double AllIncomeMultiplier { get; set; } = 1.0;
    public double ClickBonusAmount = 0;
    public int ClicksSinceLastBonus = 0; 
    
    public event System.Action<double> OnPassiveIncomeEarned;

    public int TotalClicks { get; set; } = 0;

    public event System.Action OnMoneyChanged;

    private double lastMoney = 0;

    private void Update()
    {
        if (Money > lastMoney)
        {
            double gained = Money - lastMoney;

            OnMoneyChanged?.Invoke();
            BossManager boss = FindFirstObjectByType<BossManager>();
            if (boss != null)
                boss.DamageBoss(gained); 

            lastMoney = Money;
        }
        else if (Money < lastMoney)
        {
            lastMoney = Money;
        }
    }



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InvokeRepeating(nameof(AddPassiveIncome), 1f, 1f);
    }

    private void AddPassiveIncome()
    {
        double income = MoneyPerSecond * MultiplierOverTime * AllIncomeMultiplier; 
        if (income > 0)
        {
            Money += income;
            NotifyMoneyChanged();
            OnPassiveIncomeEarned?.Invoke(income);
        }
    }



    public void NotifyMoneyChanged()
    {
        OnMoneyChanged?.Invoke();
    }
}
