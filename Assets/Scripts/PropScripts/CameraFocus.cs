using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraFocus : MonoBehaviour
{
    [SerializeField] Cinemachine.CinemachineVirtualCamera focusCamera;
    [SerializeField] bool showMouseWhileFocused = true;
    [SerializeField] bool allowUnfocusKey = true;

    public UnityEvent OnFocused = new UnityEvent();
    public UnityEvent OnUnfocused = new UnityEvent();

    const int FOCUSED_PRIORITY = 20;
    const int UNFOCUSED_PRIORITY = 0;

    bool isFocused = false;

    public void Focus()
    {
        if(isFocused == false)
        {
            PlayerCore.LocalPlayer.PlayerInput.MoveEnabled = false;
            
            isFocused = true;
            focusCamera.Priority = FOCUSED_PRIORITY;
            OnFocused.Invoke();
            if(showMouseWhileFocused)
                MouseLockHandler.Instance.ClaimMouseCursor(this);
        }
    }

    public void Unfocus()
    {
        if(isFocused)
        {
            PlayerCore.LocalPlayer.PlayerInput.MoveEnabled = true;
            isFocused = false;
            focusCamera.Priority = UNFOCUSED_PRIORITY;
            OnUnfocused.Invoke();
            if(showMouseWhileFocused)
                MouseLockHandler.Instance.ReleaseMouseCursor(this);
        }
    }

    private void Update()
    {
        //TODO move input elsewhere, keep centralized
        if(isFocused && allowUnfocusKey && Input.GetKeyDown(KeyCode.Escape))
        {
            Unfocus();
        }
    }
}
