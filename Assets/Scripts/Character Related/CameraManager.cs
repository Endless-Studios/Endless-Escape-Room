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
    public bool IsFocused => currentlyFocusedCamera != null;

    public void FocusCamera(CameraFocus cameraFocus)
    {
        if(currentlyFocusedCamera != cameraFocus)
        {
            if(currentlyFocusedCamera != null)
            {
                StartCoroutine(UnfocusCameraCoroutine());
            }
            currentlyFocusedCamera = cameraFocus;
            if(currentlyFocusedCamera != null)
            {
                PlayerHUD.Instance.SetReticleActive(false);
                PlayerCore.LocalPlayer.PlayerInput.MoveEnabled = false;
                PlayerCore.LocalPlayer.CharacterController.enabled = false;
                PlayerCore.LocalPlayer.NavMeshObstacle.enabled = false;

                currentlyFocusedCamera.CameraTarget.Priority = focusedPriority;

                if(currentlyFocusedCamera.ShowMouseWhileFocused)
                {
                    MouseLockHandler.Instance.ClaimMouseCursor(this);
                    PlayerHUD.Instance.InventoryUi.Show();
                    PlayerCore.LocalPlayer.HeldItemManager.HideProjectedVisualsAndControls();
                }

                currentlyFocusedCamera.HandledFocused();
            }
            else
            {
                PlayerHUD.Instance.SetReticleActive(true);
            }
        }
    }

    IEnumerator UnfocusCameraCoroutine()
    {
        if(currentlyFocusedCamera != null)
        {
            CameraFocus targetCamera = currentlyFocusedCamera;
            currentlyFocusedCamera = null;

            targetCamera.CameraTarget.Priority = unfocusedPriority;

            yield return null; //Maybe base off of camera transition time?
            PlayerCore.LocalPlayer.PlayerInput.MoveEnabled = true; 
            PlayerCore.LocalPlayer.CharacterController.enabled = true;
            PlayerCore.LocalPlayer.NavMeshObstacle.enabled = true;

            if(targetCamera.ShowMouseWhileFocused)
            {
                MouseLockHandler.Instance.ReleaseMouseCursor(this);
                PlayerHUD.Instance.InventoryUi.Hide();
                if(PlayerCore.LocalPlayer.ItemInspector.IsInspecting == false)
                    PlayerCore.LocalPlayer.HeldItemManager.ReenterHeld();
            }

            PlayerHUD.Instance.SetReticleActive(true);
            targetCamera.HandledUnfocused();
            targetCamera = null;
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
            StartCoroutine(UnfocusCameraCoroutine());
        }
    }
}
