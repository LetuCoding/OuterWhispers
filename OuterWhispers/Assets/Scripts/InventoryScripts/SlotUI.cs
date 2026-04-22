using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace InventoryScripts
{
    public class SlotUI : MonoBehaviour, ISelectHandler, IPointerClickHandler, ISubmitHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private Button button;

        private InventorySlot slot;
        private InventoryUI inventoryUI;

        private void Awake()
        {
            if (button == null)
                button = GetComponent<Button>();
        }

        public void Setup(InventorySlot slot, InventoryUI ui)
        {
            this.slot = slot;
            inventoryUI = ui;
            UpdateUI();
        }

        public void UpdateUI()
        {
            if (slot == null || slot.IsEmpty || slot.item == null)
            {
                icon.enabled = false;
                icon.sprite = null;
            }
            else
            {
                icon.enabled = true;
                icon.sprite = slot.item.Icon;
            }
        }

        public void OnSlotClicked()
        {
            SelectSlot();
        }

        public void OnSelect(BaseEventData eventData)
        {
            SelectSlot();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SelectSlot();

            if (EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(gameObject);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            SelectSlot();

            if (slot != null && !slot.IsEmpty && slot.item is UsableItemData)
                inventoryUI.FocusUseButton();
        }

        private void SelectSlot()
        {
            if (slot != null && !slot.IsEmpty && inventoryUI != null)
                inventoryUI.ShowDetails(slot);
        }

        public bool IsEmpty()
        {
            return slot == null || slot.IsEmpty;
        }

        public Selectable GetSelectable()
        {
            if (button == null)
                button = GetComponent<Button>();

            return button;
        }

        public InventorySlot GetSlot()
        {
            return slot;
        }
    }
}