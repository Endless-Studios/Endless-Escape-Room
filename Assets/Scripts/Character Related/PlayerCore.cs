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

    public bool IsLocalPlayer => LocalPlayer == this;
    public PlayerInput PlayerInput => playerInput;
    public CameraManager CameraManager => cameraManager;
    public InventoryBase Inventory => inventory;
    public HeldItemManager HeldItemManager => heldItemManager;

    private void Awake()
    {
        if(assignLocalPlayer)
        {
            localPlayer = this;
        }
    }
}
