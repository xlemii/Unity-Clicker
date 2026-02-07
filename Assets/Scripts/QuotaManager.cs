using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class QuotaManager : MonoBehaviour
{
    [Header("Ustawienia Quoty")]
    [SerializeField] private double baseQuota = 10;
    [SerializeField] private double currentQuota;
    [SerializeField] private Slider quotaSlider;
    [SerializeField] private TextMeshProUGUI quotaText;

    [Header("Efekt tekstowy (po osiągnięciu quoty)")]
    [SerializeField] private TextMeshProUGUI rewardTextPrefab;
    [SerializeField] private Transform rewardTextParent;
    [SerializeField] private float textFloatDistance = 50f;
    [SerializeField] private float fadeDuration = 1.5f;

    [Header("Dynamic Text Display (opcjonalny)")]
    [SerializeField] private DynamicTextDisplay textDisplay;

    private PlayerStats player;
    private bool quotaReached = false;
    private double totalEarnedSinceLastReward = 0;
    private double lastKnownMoney = 0;
    private int currentLevel = 1;

    private void Start()
    {
        player = PlayerStats.Instance;
        currentQuota = baseQuota;

        quotaSlider.maxValue = (float)currentQuota;
        quotaSlider.value = 0;
        lastKnownMoney = player.Money;

        InvokeRepeating(nameof(UpdateQuotaProgress), 0f, 0.1f);
    }

    private void UpdateQuotaProgress()
    {
        if (player == null || quotaSlider == null || quotaText == null) return;

        double moneyDiff = player.Money - lastKnownMoney;

        if (moneyDiff > 0)
            totalEarnedSinceLastReward += moneyDiff;

        lastKnownMoney = player.Money;

        quotaSlider.value = Mathf.Clamp((float)totalEarnedSinceLastReward, 0, (float)currentQuota);
        quotaText.text = $"Lvl {currentLevel} ({totalEarnedSinceLastReward:F0} / {currentQuota:F0})";

        if (!quotaReached && totalEarnedSinceLastReward >= currentQuota)
        {
            quotaReached = true;
            OnQuotaReached();
        }
    }

    private void OnQuotaReached()
    {
        if (textDisplay != null)
            textDisplay.ShowRandomText();

        if (rewardTextPrefab != null)
        {
            TextMeshProUGUI rewardText = Instantiate(rewardTextPrefab, rewardTextParent);
            rewardText.text = $"+1 Level!";
            StartCoroutine(FadeAndFloatText(rewardText));
        }
        AudioManager.Instance.Play("lvlup");
        OnRewardFinished();
    }

    private IEnumerator FadeAndFloatText(TextMeshProUGUI text)
    {
        Vector3 startPos = text.rectTransform.localPosition;
        Vector3 endPos = startPos + Vector3.up * textFloatDistance;

        float t = 0f;
        Color startColor = text.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float progress = t / fadeDuration;
            text.rectTransform.localPosition = Vector3.Lerp(startPos, endPos, progress);
            text.color = new Color(startColor.r, startColor.g, startColor.b, 1 - progress);
            yield return null;
        }

        Destroy(text.gameObject);
    }

    private void OnRewardFinished()
    {
        currentLevel++;
        totalEarnedSinceLastReward = 0;
        currentQuota *= 2;
        quotaReached = false;

        quotaSlider.maxValue = (float)currentQuota;
        quotaSlider.value = 0;

        quotaText.text = $"Lvl {currentLevel} (0 / {currentQuota:F0})";
    }
}
