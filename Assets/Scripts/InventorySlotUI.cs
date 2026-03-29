using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI quantityText;
    public Image selectionHighlight;

    public ItemSO currentItem;
    public bool isSelected;

    public bool isShopSlot = false;
    public int currentQuantity;

    public void OnClick()
    {
        if (currentItem == null) return;

        InventoryUIController.Instance.SelectedSlot(this);
    }

    public void OpenPanel()
    {
		BuyItemPanelUI.Instance.Open(currentItem);
	}

    public void SetItem(ItemSO item, int quantity)
    {
        currentItem = item;
        currentQuantity = quantity;

        icon.sprite = item.icon;
        icon.enabled = true;
        if (isShopSlot)
        {
            quantityText.text = "";
        }
        else
        {
            quantityText.text = quantity > 1 ? quantity.ToString() : "";
        }
    }

    public void ClearSlot()
    {
        currentItem = null;
        currentQuantity = 0;

        icon.sprite = null;
        icon.enabled = false;

        quantityText.text = "";

        SetSelected(false);
    }

    public void SetSelected(bool value)
    {
        isSelected = value;

        if (selectionHighlight != null)
        {
            selectionHighlight.enabled = value;
        }
    }
}