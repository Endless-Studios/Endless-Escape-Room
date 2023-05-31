using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public abstract class UiInventoryElement : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI promptText = null;
    [SerializeField] Selectable selectable = null;
    public InventorySlotBase Slot { get; private set; }

    protected RectTransform rectTransform;

    public void Initialize(InventorySlotBase slot, bool isSelectable = true)
    {
        if(selectable != null)
            selectable.interactable = isSelectable;
        Slot = slot;
        Slot.OnSlotUpdated.AddListener(HandleUpdateInternal);
        UpdatePromptText();
        rectTransform = transform as RectTransform;
        Setup();
    }

    public void HandleUpdateInternal()
    {
        UpdatePromptText();
        HandleUpdate();
    }

    protected void UpdatePromptText()
    {
        if(promptText != null)
        {
            promptText.SetText(Slot.Prompt);
        }
    }

    private void OnDestroy()
    {
        if(Slot != null)
            Slot.OnSlotUpdated.RemoveListener(HandleUpdateInternal);
    }

    protected abstract void Setup();
    public abstract void Highlight();
    public abstract void Unhighlight();
    public abstract void HandleUpdate();
}
