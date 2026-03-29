using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BuyItemPanelUI : MonoBehaviour
{
	public static BuyItemPanelUI Instance;

	public GameObject panel;
	public Image itemIcon;
	public TextMeshProUGUI itemNameText;
	public TextMeshProUGUI itemDescriptionText;
	public TextMeshProUGUI itemPriceText;
	public TMP_InputField quantityInput;

	private ItemSO currentItem;

	private void Awake()
	{
		Instance = this;
		panel.SetActive(false);
	}

	public void Open(ItemSO item)
	{
		currentItem = item;

		itemIcon.sprite = item.icon;
		itemNameText.text = item.itemName;
		itemDescriptionText.text = item.description;
		itemPriceText.text = $"Precio: {item.price}";

		quantityInput.text = "1";

		panel.SetActive(true);
	}

	public void Close()
	{
		panel.SetActive(false);
	}

	public void Buy()
	{
		int quantity = int.Parse(quantityInput.text);

		bool success = true;

		for (int i = 0; i < quantity; i++)
		{
			if (!InventoryDBManager.Instance.TryBuyItem(currentItem.id))
			{
				success = false;
				break;
			}
		}

		if (!success)
		{
			PupupManager.Instance.ShowError("No tienes suficientes monedas");
		}
		else
		{
			PupupManager.Instance.ShowMessage("Compra realizada");
		}

		CoinsManagerUI.Instance.RefreshCoins();
		InventoryDBManager.Instance.LoadInventoryItems();
		InventoryManagerUI.Instance.RefreshUI();
		Close();
	}
}
