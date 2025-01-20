using TMPro;
using UnityEngine;
using UnityEngine.Events;

//https://gamedevbeginner.com/how-to-make-countdown-timer-in-unity-minutes-seconds/

public class Timer : MonoBehaviour
{
    public float timeDefault = 10;
    public float timeRemaining;
    public bool timerIsRunning = false;
    public bool activateOnTimerComplete = false;
    public TextMeshProUGUI timeText;
    public UnityEvent OnTimerComplete;

    private void OnEnable() 
        => EventManager.Game.OnResetTimer.Get().AddListener(ResetTimer);

    private void OnDisable()
        => EventManager.Game.OnResetTimer.Get().AddListener(ResetTimer);


    // Starts the timer automatically
    private void Start()
    {
        timerIsRunning = true;
        ResetTimer();
    }

    public void ResetTimer()
        => timeRemaining = timeDefault;

    void Update()
    {
        if (!timerIsRunning)
            return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime * Time.timeScale;
            DisplayTime(timeRemaining);
        }
        else
        {
            Debug.Log("Time has run out!");
            timeRemaining = 0;
            timerIsRunning = false;

            if (!activateOnTimerComplete)
                return;

            OnTimerComplete?.Invoke();
            EventManager.Game.OnTimerComplete.Get().Invoke();
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}