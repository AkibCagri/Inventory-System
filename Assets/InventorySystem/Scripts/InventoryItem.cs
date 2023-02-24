using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField] Image image;
    [SerializeField] Image itemHandle;
    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public SOItem soItem;
    [HideInInspector] public int count = 1;
    [SerializeField] TextMeshProUGUI countText;

    public void Initialise(SOItem item)
    {
        soItem = item;
        image.sprite = soItem.image;
        RefreshCount();
    }

    public void RefreshCount()
    {
        countText.text = count.ToString();
        bool textActive = count > 1;
        countText.gameObject.SetActive(textActive);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        itemHandle.raycastTarget = false;
        InventoryManager.instance.SelectItem(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (parentAfterDrag.childCount > 0)
        {
            InventoryItem otherChildItem = parentAfterDrag.GetChild(0).GetComponent<InventoryItem>();
            count += otherChildItem.count;
            RefreshCount();

            Destroy(otherChildItem.gameObject);
        }

        transform.SetParent(parentAfterDrag);
        itemHandle.raycastTarget = true;
        SelectMe();
        InventoryManager.instance.DeselectItem();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            SelectMe();
        }
        
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            InventoryManager.instance.BreakItemFromStackToStack(this);
        }
    }

    void SelectMe()
    {
        InventorySlot inventorySlot = transform.parent.GetComponent<InventorySlot>();

        InventoryManager.instance.SelectSlot(inventorySlot);
    }
}
