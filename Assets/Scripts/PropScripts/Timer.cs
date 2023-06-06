using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A timer used to trigger events on a delay. Can be looped or paused
/// </summary>
public class Timer : MonoBehaviour
{
    private enum TimerState
    {
        Stopped,
        Playing,
        Paused
    }

    [SerializeField] float duration = 30;
    [SerializeField] bool autoStart = true;
    [SerializeField] bool loop = false;

    public UnityEvent OnTimerFinished = new UnityEvent();

    TimerState currentState = TimerState.Stopped;
    Coroutine timerCoroutine = null;

    void Awake()
    {
        if(autoStart)
            PlayTimer();
    }

    /// <summary>
    /// Starts the timer from a stopped, or paused state
    /// </summary>
    public void PlayTimer()
    {
        if(currentState == TimerState.Stopped)
        {
            timerCoroutine = StartCoroutine(TimerCoroutine());
        }
        else if(currentState == TimerState.Paused)
        {
            currentState = TimerState.Playing;
        }
    }

    /// <summary>
    /// Pauses the timer if playing, unpauses if paused
    /// </summary>
    public void TogglePause()
    {
        if(currentState == TimerState.Playing)
        {
            currentState = TimerState.Paused;
        }
        else if(currentState == TimerState.Paused)
        {
            currentState = TimerState.Playing;
        }
    }

    /// <summary>
    /// Pauses the timer, if playing
    /// </summary>
    public void PauseTimer()
    {
        if(currentState == TimerState.Playing)
        {
            currentState = TimerState.Paused;
        }
    }

    /// <summary>
    /// Stops the timer, if playing or paused
    /// </summary>
    public void StopTimer()
    {
        if(currentState == TimerState.Playing || currentState == TimerState.Paused)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
    }

    /// <summary>
    /// Will resume the timer only if it is paused, not stopped
    /// </summary>
    public void ResumeTimer()
    {
        if(currentState == TimerState.Paused)
        {
            currentState = TimerState.Playing;
        }
    }

    /// <summary>
    /// Sets the duration to a new value. Note that if currently running this can trigger an immediate completion if less than the elapsed time.
    /// </summary>
    /// <param name="newDuration">The new length of the timer</param>
    public void SetDuration(float newDuration)
    {
        //Ensure that duration cannot go negative
        duration = Mathf.Max(0, newDuration);
    }

    /// <summary>
    /// Sets the duration to a new value. Note that if currently running this can trigger an immediate completion if less than the elapsed time.
    /// </summary>
    /// <param name="delta">The amount by which to change it</param>
    public void ModifyDuration(float delta)
    {
        //Ensure that duration cannot go negative
        duration = Mathf.Max(0, duration + delta);
    }

    IEnumerator TimerCoroutine()
    {
        currentState = TimerState.Playing;
        float elapsedTime = 0;
        do
        {
            yield return null;

            while(elapsedTime < duration)
            {//Wait until we reach our duration
                if(currentState == TimerState.Playing)
                    elapsedTime += Time.deltaTime;
                yield return null;
            }
            OnTimerFinished.Invoke();
            //Reset elapsed time in case it loops. Subtract rather than setting to 0 for more accuracy after many loops
            while(elapsedTime > duration) //Incase duration was modified to a lower value, avoid several simultaneous finishes at once.
                elapsedTime -= duration; 
        } while(loop); // if we're looping, just keep playing!

        yield return null;
        currentState = TimerState.Stopped;
        timerCoroutine = null;
    }
}
