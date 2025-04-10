using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviourSingleton<PlayerHUD>
{
    [Header("Interact Screen")]
    [SerializeField] GameObject interactScreen;
    [SerializeField] GameObject interactPrompt;
    [SerializeField] GameObject reticleObject;
    [SerializeField] TextMeshProUGUI interactText;

    [Header("Inspect Screen")]
    [SerializeField] GameObject inspectScreen;
    [SerializeField] GameObject inspectTakePrompt;
    [SerializeField] GameObject inspectReturnPrompt;

    [Header("Held Screen")]
    [SerializeField] GameObject heldScreen;
    [SerializeField] GameObject heldInspectPrompt;
    [SerializeField] GameObject helpDropPrompt;
    [SerializeField] GameObject heldUsePrompt;

    [Header("UI Class References")]
    [SerializeField] InventoryUI inventoryUi;

    [Header("Fade To Black")]
    [SerializeField] FadeToBlack fadeToBlack;

    public InventoryUI InventoryUi => inventoryUi;
    public FadeToBlack FadeToBlack => fadeToBlack;

    protected override void Awake()
    {
        base.Awake();
        SetInteractText(string.Empty);
        SetInteractScreenActive(true);
        SetInspectScreenActive(false);
        SetHeldScreenActive(false);
    }

    public void SetInteractText(string newText)
    {
        interactText.SetText(newText);
        if(string.IsNullOrEmpty(newText))
            interactPrompt.SetActive(false);
        else
            interactPrompt.SetActive(true);
    }

    public void SetInteractScreenActive(bool active)
    {
        interactScreen.SetActive(active);
    }

    public void SetInspectScreenActive(bool active, bool canPickup = true)
    {
        inspectScreen.SetActive(active);
        inspectTakePrompt.SetActive(canPickup);
    }

    public void SetHeldScreenActive(bool active, bool drop = true)
    {
        heldScreen.SetActive(active);
        heldUsePrompt.SetActive(!drop);
        helpDropPrompt.SetActive(drop);
    }

    public void SetReticleActive(bool active)
    {
        reticleObject.SetActive(active);
    }
}
