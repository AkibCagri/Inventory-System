using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInventoryItem", menuName = "ScriptableObjects/InventoryItem")]
public class SOItem : ScriptableObject
{
    public Sprite image;
    public int stackCount = 1;
    public UseType useType;

    public List<ItemUsingEffects> usingEffects = new List<ItemUsingEffects>();
    public List<int> effectAmounts = new List<int>();


    public enum UseType
    {
        notUseable,
        use,
        eat,
        equip
    }

    public enum ItemUsingEffects
    {
        noEffect,
        heal,
        feed,
        quench,
        increaseArmor,
        equip
    }
}
