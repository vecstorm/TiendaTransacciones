using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public ItemSO item;
    public int quantity;

    public InventoryItem(ItemSO item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    } 

}
