using System.Collections;
using System.Collections.Generic;
using Ai;
using UnityEngine;
using UnityEngine.AI;

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
    [SerializeField] HidingComponent hidingComponent;
    [SerializeField] HealthComponent healthComponent;
    [SerializeField] CharacterController characterController;
    [SerializeField] NavMeshObstacle navMeshObstacle;
    [SerializeField] PlayerTarget playerTarget;
    [SerializeField] Rigidbody playerRigidbody;

    public bool IsLocalPlayer => LocalPlayer == this;
    public PlayerInput PlayerInput => playerInput;
    public CameraManager CameraManager => cameraManager;
    public InventoryBase Inventory => inventory;
    public HeldItemManager HeldItemManager => heldItemManager;
    public Transform FpsCameraRootTransform => fpsCameraRootTransform;
    public ItemInspector ItemInspector => itemInspector;
    public HidingComponent HidingComponent => hidingComponent;
    public HealthComponent HealthComponent => healthComponent;
    public CharacterController CharacterController => characterController;
    public NavMeshObstacle NavMeshObstacle => navMeshObstacle;
    public PlayerTarget PlayerTarget => playerTarget;
    public Rigidbody Rigidbody => playerRigidbody;

    private void Awake()
    {
        if(assignLocalPlayer)
        {
            localPlayer = this;
        }
    }
}
