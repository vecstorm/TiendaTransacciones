using UnityEngine;

public class InventorySellManager : MonoBehaviour
{
	public static InventorySellManager Instance;

	[Range(0f, 1f)]
	public float sellMultiplier = 0.5f; // 50% del precio original

	private void Awake()
	{
		Instance = this;
	}

	/// <summary>
	/// Vende una unidad del item seleccionado
	/// </summary>
	public void SellOne(int itemID)
	{
		ItemSO item = ItemDatabase.instance.GetItemByID(itemID);
		if (item == null) return;

		int sellPrice = Mathf.FloorToInt(item.price * sellMultiplier);

		bool success = InventoryDBManager.Instance.SellItem_Transaction(itemID, 1, sellPrice);

		if (!success)
		{
			PupupManager.Instance.ShowError("Error al vender el item");
			return;
		}

		InventoryManagerUI.Instance.RefreshUI();
		CoinsManagerUI.Instance.RefreshCoins();
		PupupManager.Instance.ShowMessage($"Has vendido 1 {item.itemName} por {sellPrice} monedas");
	}

	/// <summary>
	/// Vende toda la pila del item seleccionado
	/// </summary>
	public void SellAll(int itemID)
	{
		InventoryItem invItem = InventoryDBManager.Instance.inventory.Find(i => i.item.id == itemID);
		if (invItem == null) return;

		ItemSO item = invItem.item;
		int quantity = invItem.quantity;
		int sellPrice = Mathf.FloorToInt(item.price * sellMultiplier) * quantity;

		bool success = InventoryDBManager.Instance.SellItem_Transaction(itemID, quantity, sellPrice);

		if (!success)
		{
			PupupManager.Instance.ShowError("Error al vender el item");
			return;
		}

		InventoryManagerUI.Instance.RefreshUI();
		CoinsManagerUI.Instance.RefreshCoins();
		PupupManager.Instance.ShowMessage($"Has vendido {quantity} {item.itemName} por {sellPrice} monedas");
	}
}
