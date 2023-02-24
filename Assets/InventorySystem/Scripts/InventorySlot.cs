using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    [SerializeField] Image image;
    [SerializeField] Color selectedColor, deselectedColor;
    [HideInInspector] public bool isSelected;

    public bool isEquipSlot;

    public void SelectMe(bool selectionStatus)
    {
        isSelected = selectionStatus;
        if (isSelected)
        {
            image.color = selectedColor;
        }
        else
        {
            image.color = deselectedColor;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();

        if (isEquipSlot && inventoryItem.soItem.useType != SOItem.UseType.equip)
            return;

        if (transform.childCount != 0)
        {
            InventoryItem childItem = transform.GetChild(0).GetComponent<InventoryItem>();
            if (inventoryItem.soItem != childItem.soItem || childItem.count + inventoryItem.count > inventoryItem.soItem.stackCount || childItem.soItem.stackCount <= 1)
                return;
        }

        inventoryItem.parentAfterDrag = transform;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            InventoryManager.instance.BreakItemFromStackToSlot(this);
        }
    }
}
