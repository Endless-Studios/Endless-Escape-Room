using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Openable : MonoBehaviour
{
    [SerializeField] bool isOpen = false;
    [SerializeField] bool isLocked = false;

    public UnityEvent OnOpened = new UnityEvent();
    public UnityEvent OnOpenFailed = new UnityEvent();
    public UnityEvent OnClosed = new UnityEvent();
    public UnityEvent OnLocked = new UnityEvent();
    public UnityEvent OnUnlocked = new UnityEvent();

    public bool IsLocked { get => isLocked; }
    public bool IsOpen => isOpen;

    /// <summary>
    /// Attempts to open the door
    /// </summary>
    /// <returns>Did the door successfully open?</returns>
    public bool Open()
    {
        if(!isOpen && !isLocked)
        {
            isOpen = true;
            OnOpened.Invoke();
            return true;
        }
        else
        {
            OnOpenFailed.Invoke();
            return false;
        }
    }

    public void Close()
    {
        if(isOpen)
        {
            isOpen = false;
            OnClosed.Invoke();
        }
    }

    /// <summary>
    /// If open, will close. If closed, will open (if not locked)
    /// </summary>
    public void ToggleOpen()
    {
        if(isOpen)
            Close();
        else
            Open();
    }

    public void Unlock()
    {
        if(isLocked)
        {
            isLocked = false;
            OnUnlocked.Invoke();
        }
    }

    public void Lock()
    {
        if(!isLocked)
        {
            isLocked = true;
            OnLocked.Invoke();
        }
    }

    public void ToggleLocked()
    {
        if(isLocked)
            Unlock();
        else
            Lock();
    }
}
