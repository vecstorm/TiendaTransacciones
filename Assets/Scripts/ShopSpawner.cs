
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemSO;

public class ShopSpawner : MonoBehaviour
{
    public InventorySlotUI weaponSlot;
    public InventorySlotUI ammoSlot;
    public InventorySlotUI foodSlot;


    // Start is called before the first frame update
    void Start()
    {
        RefreshShop();
    }

    private void RefreshShop()
    {
        weaponSlot.SetItem(GetRandomItem(ItemType.Weapon), 1);
        ammoSlot.SetItem(GetRandomItem(ItemType.Ammo), 1);
        foodSlot.SetItem(GetRandomItem(ItemType.Food), 1);

    }

    private ItemSO GetRandomItem(ItemType type)
    {
        var items = ItemDatabase.instance.items.FindAll(i => i.type == type);
        return items[Random.Range(0,items.Count)];
    }
}
