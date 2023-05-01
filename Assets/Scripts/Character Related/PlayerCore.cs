using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCore : MonoBehaviour
{
    static PlayerCore localPlayer = null;
    public static PlayerCore LocalPlayer => localPlayer;

    [SerializeField] bool assignLocalPlayer = true;
    [SerializeField] PlayerInput playerInput;

    public bool IsLocalPlayer => LocalPlayer == this;
    public PlayerInput PlayerInput => playerInput;

    private void Awake()
    {
        if(assignLocalPlayer)
        {
            localPlayer = this;
        }
    }
}
