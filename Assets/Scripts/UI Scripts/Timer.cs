using TMPro;
using UnityEngine;
using UnityEngine.Events;

//https://gamedevbeginner.com/how-to-make-countdown-timer-in-unity-minutes-seconds/

public class Timer : MonoBehaviour
{
    public float timeDefault = 10;
    public float timeRemaining;
    public bool timerIsRunning = false;
    public TextMeshProUGUI timeText;
    public UnityEvent OnTimerComplete;


    private void Start()
    {
        timeRemaining = timeDefault;
        DisableTimer();
    }

    public void DisableTimer()
    {
        timerIsRunning = false;
        timeText.gameObject.SetActive(false);
    }

    public void EnableTimer(float time)
    {
        timerIsRunning = true;
        timeText.gameObject.SetActive(true);

        timeRemaining = time;
    }

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

            OnTimerComplete?.Invoke();
            EventManager.Game.OnTimerComplete.Get().Invoke();
            EventManager.ChatBubble.OnAddChat.Get().Invoke(new ChatMessageGroup() { messages = new() { new ChatMessage() { name ="Customer", message="I'm not paying you, we're late." } } });

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