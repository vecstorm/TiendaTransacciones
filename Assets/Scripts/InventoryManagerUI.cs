
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManagerUI : MonoBehaviour
{
    public static InventoryManagerUI Instance;

    public GameObject slotPrefab;
    public Transform slotContainer;
    private Image icon;

    public List<InventorySlotUI> slots = new List<InventorySlotUI>();


    private void Awake()
    {
        Instance = this;
        icon = GetComponentInChildren<Image>();
    }

    /// <summary>
    /// Metodo que usamos para rellenar los items en el inventario.
    /// </summary>
    public void RefreshUI()
    {
        ClearSlots();

        foreach(var invItem in InventoryDBManager.Instance.inventory)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotContainer);
            InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();

            slotUI.SetItem(invItem.item, invItem.quantity);
            slots.Add(slotUI);
        }
    }

    /// <summary>
    /// Metodo para limpiar todo el inventario
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void ClearSlots()
    {
        foreach(Transform child in slotContainer)
            Destroy(child.gameObject);

        slots.Clear();
    }
}
