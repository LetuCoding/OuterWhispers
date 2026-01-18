using UnityEngine;
using UnityEngine.UI;

namespace InventoryScripts
{
    public class SlotUI : MonoBehaviour
    {
        [SerializeField] private Image icon;
        private InventorySlot slot;
        private InventoryUI inventoryUI;
        public void Setup(InventorySlot slot, InventoryUI ui)
        {
            this.slot = slot;
            inventoryUI = ui;
        }


        public void UpdateUI()
        {
            if (slot.IsEmtpy)
            {
                icon.enabled = false;
            }
            else
            {
                icon.enabled = true;
                icon.sprite = slot.item.Icon;
            }
        }

        public void OnSlotClicked()
        {
            if (!slot.IsEmtpy)
            {
                inventoryUI.ShowDetails(slot);
            }
        }
    }
}