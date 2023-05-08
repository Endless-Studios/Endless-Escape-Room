using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] int focusedPriority = 20;
    [SerializeField] int unfocusedPriority = 0;

    CameraFocus currentlyFocusedCamera = null;
    bool IsFocused => currentlyFocusedCamera != null;

    public void FocusCamera(CameraFocus cameraFocus)
    {
        if(currentlyFocusedCamera != cameraFocus)
        {
            if(currentlyFocusedCamera != null)
            {
                UnfocusCamera();
            }
            currentlyFocusedCamera = cameraFocus;
            if(currentlyFocusedCamera != null)
            {
                PlayerCore.LocalPlayer.PlayerInput.MoveEnabled = false;
                currentlyFocusedCamera.CameraTarget.Priority = focusedPriority;

                if(currentlyFocusedCamera.ShowMouseWhileFocused)
                {
                    MouseLockHandler.Instance.ClaimMouseCursor(this);
                    PlayerHUD.Instance.InventoryUi.Show();
                    PlayerCore.LocalPlayer.HeldItemManager.HideProjectedVisualsAndControls();
                }

                currentlyFocusedCamera.HandledFocused();
            }
        }
    }

    private void UnfocusCamera()
    {
        if(currentlyFocusedCamera != null)
        {
            PlayerCore.LocalPlayer.PlayerInput.MoveEnabled = true;
            currentlyFocusedCamera.CameraTarget.Priority = unfocusedPriority;

            if(currentlyFocusedCamera.ShowMouseWhileFocused)
            {
                MouseLockHandler.Instance.ReleaseMouseCursor(this);
                PlayerHUD.Instance.InventoryUi.Hide();
                PlayerCore.LocalPlayer.HeldItemManager.ReenterHeld();
            }

            currentlyFocusedCamera.HandledUnfocused();
            currentlyFocusedCamera = null;
        }
    }

    public void UnfocusCamera(CameraFocus cameraFocus)
    {
        if(currentlyFocusedCamera == cameraFocus)
        {
            FocusCamera(null);
        }
    }

    private void Update()
    {
        if(IsFocused && currentlyFocusedCamera.AllowUnfocusKey && PlayerCore.LocalPlayer.PlayerInput.GetBackPressed())
        {
            UnfocusCamera();
        }
    }
}
