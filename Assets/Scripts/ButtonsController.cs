using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsController : MonoBehaviour
{

    public void BuySelectedItem()
    {
		var slot = InventoryUIController.Instance.selectedSlot;
		if (slot == null || slot.currentItem == null) return;

		bool success = InventoryDBManager.Instance.TryBuyItem(slot.currentItem.id);

		if (!success)
		{
			PupupManager.Instance.ShowError("No tienes suficientes monedas");
			return;
		}

		PupupManager.Instance.ShowMessage("Compra realizada");
		InventoryManagerUI.Instance.RefreshUI();
		CoinsManagerUI.Instance.RefreshCoins();
	}

    public void RemoveSelectedFromInventory()
    {

        var slot = InventoryUIController.Instance.selectedSlot;
        if (slot == null || slot.currentItem == null) return;

        InventoryDBManager.Instance.RemoveItemFull(slot.currentItem.id);
    }

    public void IncreaseQuantity()
    {
        var slot = InventoryUIController.Instance.selectedSlot;
        if (slot == null || slot.currentItem == null) return;
        InventoryDBManager.Instance.AddItem(slot.currentItem.id, 1);
    }
    public void DecreaseQuantity()
    {
        var slot = InventoryUIController.Instance.selectedSlot;
        if (slot == null || slot.currentItem == null) return;
        InventoryDBManager.Instance.RemoveItem(slot.currentItem.id, 1);
    }

	public void SellSelectedItem()
	{
		var slot = InventoryUIController.Instance.selectedSlot;
		if (slot == null || slot.currentItem == null) return;

		InventorySellManager.Instance.SellAll(slot.currentItem.id);
	}

}
