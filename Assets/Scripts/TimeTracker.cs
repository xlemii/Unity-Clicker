using UnityEngine;
using TMPro;

public class TimeTracker : MonoBehaviour
{
    [Header("Ustawienia UI")]
    [SerializeField] private TextMeshProUGUI timeText;

    private float elapsedTime = 0f;
    private bool isRunning = true;

    private void Update()
    {
        if (!isRunning) return;

        elapsedTime += Time.deltaTime;

        if (timeText != null)
            timeText.text = "TIME: " + FormatTime(elapsedTime);
    }

    private string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60);
        int secs = Mathf.FloorToInt(seconds % 60);
        return $"{minutes:00}:{secs:00}";
    }

    public void PauseTimer()
    {
        isRunning = false;
    }

    public void ResumeTimer()
    {
        isRunning = true;
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        if (timeText != null)
            timeText.text = "TIME: " + FormatTime(0);
    }
}
