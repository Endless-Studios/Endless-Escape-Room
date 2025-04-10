using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraFocus : MonoBehaviour
{
    [SerializeField] Cinemachine.CinemachineVirtualCamera focusCamera;
    [SerializeField] bool showMouseWhileFocused = true;
    [SerializeField] bool allowUnfocusKey = true;
    [SerializeField] bool priorityFocus = false;

    public UnityEvent OnFocused = new UnityEvent();
    public UnityEvent OnUnfocused = new UnityEvent();

    public CinemachineVirtualCamera CameraTarget => focusCamera;
    public bool ShowMouseWhileFocused => showMouseWhileFocused;
    public bool AllowUnfocusKey
    {
        get => allowUnfocusKey;
        set => allowUnfocusKey = value;
    }

    public bool PriorityFocus => priorityFocus;

    internal void HandledFocused()
    {
        OnFocused.Invoke();
    }

    internal void HandledUnfocused()
    {
        OnUnfocused.Invoke();
    }

    /// <summary>
    /// Exposed to be triggered via events. Note that camera focus can fail
    /// </summary>
    public void Focus()
    {
        PlayerCore.LocalPlayer.CameraManager.FocusCamera(this);
    }

    /// <summary>
    /// Attempts to focus the camera
    /// </summary>
    /// <returns>Was the focus successful</returns>
    public bool AttemptFocus()
    {
        return PlayerCore.LocalPlayer.CameraManager.FocusCamera(this);
    }

    public void Unfocus()
    {
        PlayerCore.LocalPlayer.CameraManager.UnfocusCamera(this);
    }
}
