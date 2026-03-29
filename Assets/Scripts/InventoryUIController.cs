using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUIController : MonoBehaviour
{
    public static InventoryUIController Instance;
    public InventorySlotUI selectedSlot;

    private void Awake()
    {
        Instance = this;
    }

    public void SelectedSlot(InventorySlotUI slot)
    {
        if (selectedSlot != null)
            selectedSlot.SetSelected(false);

        selectedSlot = slot;
        slot.SetSelected(true);
    }
}
