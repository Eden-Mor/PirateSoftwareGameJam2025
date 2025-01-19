using UnityEngine;
using UnityEngine.Events;

public class PauseSystem : MonoBehaviour
{
    public UnityEvent OnPause;
    public UnityEvent OnResume;

    bool isPaused;

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        isPaused = !isPaused;
        PauseUnpause();
    }

    private void PauseUnpause()
    {
        Time.timeScale = isPaused ? 0 : 1;
        (isPaused
            ? OnPause
            : OnResume
            )?.Invoke();
    }

    public bool GetIsPaused() 
        => isPaused;

    public void Pause()
    {
        isPaused = true;
        PauseUnpause();
    }

    public void Resume()
    {
        isPaused = false;
        PauseUnpause();
    }
}
