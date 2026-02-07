using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BossManager : MonoBehaviour
{
    [Header("Timer Settings")]
    public float bossSpawnTime = 600f;
    private float timeRemaining;
    private bool bossActive = false;

    [Header("UI Elements")]
    public TextMeshProUGUI timerText;
    public List<GameObject> uiToHide = new List<GameObject>(); 
    public GameObject bossUI;

    [Header("Boss Stats")]
    public double maxHealth = 6767676767d; 
    private double currentHealth;

    [Header("Boss UI Elements")]
    public Slider bossHealthSlider;
    public TextMeshProUGUI bossHealthText;

    [Header("End Screen")]
    public GameObject endScreenUI;


    [Header("Visuals")]
    public Transform bossVisual; 
    public float hitScaleMultiplier = 1.2f;
    public float hitScaleDuration = 0.15f; 
    
    [Header("Audio")]
    public AudioSource musicSource; 
    public float fadeOutDuration = 2f; 


    private Vector3 originalScale;
    private bool isScaling = false;

    private void Start()
    {
        timeRemaining = bossSpawnTime;
        currentHealth = maxHealth;

        if (bossUI != null)
            bossUI.SetActive(false);

        if (bossVisual != null)
            originalScale = bossVisual.localScale;

        if (PlayerStats.Instance != null)
            PlayerStats.Instance.OnMoneyChanged += OnPlayerEarnedMoney;
    }

    private void OnDestroy()
    {
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.OnMoneyChanged -= OnPlayerEarnedMoney;
    }

    private void Update()
    {
        if (!bossActive)
        {
            HandleTimer();
        }
        else
        {
            UpdateBossUI();
        }
    }

    private void HandleTimer()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerUI(timeRemaining);
        }
        else
        {
            ActivateBoss();
        }
    }

    private void UpdateTimerUI(float time)
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);

            timerText.text = $"{minutes:00}:{seconds:00}";

            if (time <= 60f)
                timerText.color = Color.red;
            else
                timerText.color = Color.black;
        }
    }
    
    private IEnumerator FadeOutMusic()
    {
        float startVolume = musicSource.volume;

        while (musicSource.volume > 0f)
        {
            musicSource.volume -= startVolume * Time.deltaTime / fadeOutDuration;
            yield return null;
        }

        musicSource.volume = 0f;
        musicSource.Stop();
    }

    private void ActivateBoss()
    {
        if (musicSource != null)
        StartCoroutine(FadeOutMusic());
        bossActive = true;
        AudioManager.Instance.Play("boss");
        timeRemaining = 0f;

        foreach (GameObject ui in uiToHide)
        {
            if (ui != null)
                ui.SetActive(false);
        }

        if (bossUI != null)
            bossUI.SetActive(true);

        if (timerText != null)
        {
            timerText.text = "DEFEAT BOSS";
            timerText.color = Color.red;
        }

        UpdateBossUI();
    }

    private void UpdateBossUI()
    {
        if (bossHealthSlider != null)
        {
            float fillPercent = (float)(currentHealth / maxHealth);
            bossHealthSlider.value = bossHealthSlider.maxValue * fillPercent;
        }

        if (bossHealthText != null)
        {
            bossHealthText.text = $"HP: {currentHealth:n0} / {maxHealth:n0}";
        }
    }



    private void OnPlayerEarnedMoney()
    {

    }

    public void DamageBoss(double amount)
    {
        if (!bossActive) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp((float)currentHealth, 0, (float)maxHealth);

        UpdateBossUI();

        if (bossVisual != null)
            StartCoroutine(HitScaleEffect());

        if (currentHealth <= 0)
        {
            BossDefeated();
        }
    }

    private IEnumerator HitScaleEffect()
    {
        if (isScaling) yield break;
        isScaling = true;

        bossVisual.localScale = originalScale * hitScaleMultiplier;

        yield return new WaitForSeconds(hitScaleDuration);

        bossVisual.localScale = originalScale;
        isScaling = false;
    }
    
    private IEnumerator WaitForExitClick()
    {
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }


    private bool bossDefeated = false;

    private void BossDefeated()
    {
        if (bossDefeated)
            return;

        bossDefeated = true;
        AudioManager.Instance.Stop("boss");

        if (bossUI != null)
            bossUI.SetActive(false);

        if (endScreenUI != null)
            endScreenUI.SetActive(true);
        AudioManager.Instance.Play("ending");

        StartCoroutine(WaitForExitClick());
    }


}
