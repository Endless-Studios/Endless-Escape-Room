using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCore : MonoBehaviour
{
    static PlayerCore localPlayer = null;
    public static PlayerCore LocalPlayer => localPlayer;

    [SerializeField] bool assignLocalPlayer = true;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] CameraManager cameraManager;
    [SerializeField] InventoryBase inventory;
    [SerializeField] HeldItemManager heldItemManager;
    [SerializeField] ItemInspector itemInspector;
    [SerializeField] Transform fpsCameraRootTransform;

    public bool IsLocalPlayer => LocalPlayer == this;
    public PlayerInput PlayerInput => playerInput;
    public CameraManager CameraManager => cameraManager;
    public InventoryBase Inventory => inventory;
    public HeldItemManager HeldItemManager => heldItemManager;
    public Transform FpsCameraRootTransform => fpsCameraRootTransform;
    public ItemInspector ItemInspector => itemInspector;

    private void Awake()
    {
        if(assignLocalPlayer)
        {
            localPlayer = this;
        }
    }
}
