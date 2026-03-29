using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item")]
public class ItemSO : ScriptableObject
{
    public int id;
    public string itemName;
    public int price;
    public int MaxStack;
    public string description;
    public Sprite icon;

    public enum ItemType {  Weapon, Ammo, Food, Money }

    public ItemType type;

}
