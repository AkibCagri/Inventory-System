using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [SerializeField] InventorySlot[] inventorySlots;
    [SerializeField] InventorySlot[] equipSlots;
    [SerializeField] GameObject inventoryItemPrefab;

    InventorySlot selectedSlot;
    InventoryItem selectedItem;

    [SerializeField] Image selectedItemImage;
    [SerializeField] GameObject UseButton, DropButton;
    [SerializeField] GameObject noSelectionText;

    [SerializeField] List<string> useButtonInfoList = new List<string>();


    [Header("Test")]
    [SerializeField] SOItem stackableItem;
    [SerializeField] SOItem notStackableItem;
    [SerializeField] SOItem notUseableItem;
    [SerializeField] SOItem equipInventoryItem;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Update()
    {
        // test
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AddItem(stackableItem);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            AddItem(notStackableItem);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            AddItem(notUseableItem);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            AddItem(equipInventoryItem);
        }
    }

    public void SelectItem(InventoryItem item)
    {
        selectedItem = item;
    }

    public void DeselectItem()
    {
        selectedItem = null;
    }

    public void SelectSlot(InventorySlot slot)
    {
        if (selectedSlot != null)
        {
            if (selectedSlot != slot)
            {
                selectedSlot.SelectMe(false);
            }
        }

        selectedSlot = slot;
        selectedSlot.SelectMe(true);

        InventoryItem InventoryItem = selectedSlot.transform.GetChild(0).GetComponent<InventoryItem>();
        SOItem soItem = InventoryItem.soItem;
        selectedItemImage.sprite = soItem.image;
        selectedItemImage.gameObject.SetActive(true);
        noSelectionText.gameObject.SetActive(false);
        UseButton.SetActive(true);
        DropButton.SetActive(true);

        string useButtontext = "";
        switch (soItem.useType)
        {
            case SOItem.UseType.notUseable:
                UseButton.SetActive(false);
                break;
            case SOItem.UseType.use:
                useButtontext = useButtonInfoList[0];
                break;
            case SOItem.UseType.eat:
                useButtontext = useButtonInfoList[1];
                break;
            case SOItem.UseType.equip:
                bool canShowButton = true;
                foreach (var item in equipSlots)
                {
                    if (slot == item)
                    {
                        canShowButton = false;
                    }
                }
                if (canShowButton)
                    useButtontext = useButtonInfoList[2];
                else
                    UseButton.SetActive(false);
                break;
        }

        UseButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = useButtontext;
    }

    public bool AddItem(SOItem item)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.soItem == item && itemInSlot.count < item.stackCount && item.stackCount > 1)
            {
                itemInSlot.count++;
                itemInSlot.RefreshCount();
                return true;
            }
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                SpawnNewItem(item, slot);
                return true;
            }
        }

        return false;
    }

    public bool BreakItemFromStackToSlot(InventorySlot slot)
    {
        if (selectedItem == null)
            return false;

        if (slot.isEquipSlot && selectedItem.soItem.useType != SOItem.UseType.equip)
            return false;

        if (selectedItem.count > 1)
        {
            selectedItem.count--;
            selectedItem.RefreshCount();

            SpawnNewItem(selectedItem.soItem, slot);

            return true;
        }
        return false;
    }

    public bool BreakItemFromStackToStack(InventoryItem stack)
    {
        if (selectedItem == null)
            return false;

        if (stack.soItem == selectedItem.soItem && stack.count < selectedItem.soItem.stackCount && selectedItem.soItem.stackCount > 1 && selectedItem.count > 1)
        {
            stack.count++;
            stack.RefreshCount();

            selectedItem.count--;
            selectedItem.RefreshCount();

            return true;
        }

        return false;
    }



    void SpawnNewItem(SOItem item, InventorySlot slot)
    {
        GameObject newItem = Instantiate(inventoryItemPrefab, slot.transform);
        newItem.GetComponent<InventoryItem>().Initialise(item);
    }

    public void UseButtonEffect()
    {
        if (selectedSlot == null)
            return;
        InventoryItem inventoryItem = selectedSlot.transform.GetChild(0).GetComponent<InventoryItem>();
        SOItem soItem = inventoryItem.soItem;

        for (int i = 0; i < soItem.usingEffects.Count; i++)
        {
            switch (soItem.usingEffects[i])
            {
                case SOItem.ItemUsingEffects.feed:
                    Debug.Log("Feed +" + soItem.effectAmounts[i]);
                    break;
                case SOItem.ItemUsingEffects.noEffect:
                    Debug.Log("No effect +" + soItem.effectAmounts[i]);
                    break;
                case SOItem.ItemUsingEffects.quench:
                    Debug.Log("Quench +" + soItem.effectAmounts[i]);
                    break;
                case SOItem.ItemUsingEffects.increaseArmor:
                    Debug.Log("Increase armor +" + soItem.effectAmounts[i]);
                    break;
                case SOItem.ItemUsingEffects.heal:
                    Debug.Log("Heal +" + soItem.effectAmounts[i]);
                    break;
                case SOItem.ItemUsingEffects.equip:
                    Debug.Log("Equip ");
                    bool isEquiped = Equip(inventoryItem);
                    if (isEquiped == false)
                    {
                        Debug.Log("Equip slots are not empty");
                    }
                    return;
            }
        }

        RemoveItem(selectedSlot.transform.GetChild(0).gameObject);
    }

    bool Equip(InventoryItem equipItem)
    {
        foreach (var item in equipSlots)
        {
            if (item.transform.childCount == 0)
            {
                equipItem.transform.SetParent(item.transform);
                SelectSlot(equipItem.transform.parent.GetComponent<InventorySlot>());
                return true;
            }
        }

        return  false;
    }

    public void DropButtonEffect()
    {
        if (selectedSlot == null)
            return;

        Debug.Log("Item is dropped");
        GameObject droppedItem = selectedSlot.transform.GetChild(0).gameObject;
        RemoveItem(droppedItem);
    }

    public void RemoveItem(GameObject droppedItem)
    {
        if(droppedItem.GetComponent<InventoryItem>().count > 1)
        {
            droppedItem.GetComponent<InventoryItem>().count--;
            droppedItem.GetComponent<InventoryItem>().RefreshCount();
        }
        else
        {
            Destroy(droppedItem);
            selectedSlot.SelectMe(false);
            selectedItemImage.gameObject.SetActive(false);
            noSelectionText.gameObject.SetActive(true);
            UseButton.SetActive(false);
            DropButton.SetActive(false);
        }
    }
}
