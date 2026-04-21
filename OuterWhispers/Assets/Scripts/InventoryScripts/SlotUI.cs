using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace InventoryScripts
{
    public class SlotUI : MonoBehaviour, ISelectHandler, IPointerClickHandler, ISubmitHandler
    {
        [SerializeField] private Image icon;

        private InventorySlot slot;
        private InventoryUI inventoryUI;
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();

            if (button != null)
            {
                button.navigation = Navigation.defaultNavigation;
            }
        }

        public void Setup(InventorySlot slot, InventoryUI ui)
        {
            this.slot = slot;
            inventoryUI = ui;
            UpdateUI();
        }

        public void UpdateUI()
        {
            if (slot == null || slot.IsEmpty)
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
            if (slot != null && !slot.IsEmpty)
            {
                inventoryUI.ShowDetails(slot);
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (slot != null && !slot.IsEmpty)
            {
                inventoryUI.ShowDetails(slot);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (slot != null && !slot.IsEmpty)
            {
                inventoryUI.ShowDetails(slot);
                EventSystem.current.SetSelectedGameObject(gameObject);
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (slot != null && !slot.IsEmpty)
            {
                inventoryUI.ShowDetails(slot);
            }
        }

        public bool IsEmpty()
        {
            return slot == null || slot.IsEmpty;
        }
    }
}