using InventoryScripts;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("Inventory")] [SerializeField] private Inventory inventory;
    [SerializeField] private Transform slotsParent;
    [SerializeField] private SlotUI slotPrefab;

    [Header("Details Panel")] [SerializeField]
    private Image detailIcon;

    [SerializeField] private TMP_Text detailName;
    [SerializeField] private TMP_Text detailDescription;
    [SerializeField] private Button useButton;

    private InventorySlot selectedSlot;
    [SerializeField] private GameObject inventoryRoot;

    private void Start()
    {
        BuildSlots();
        ClearDetails();
        inventoryRoot.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleInventory();
        }
    }


    void BuildSlots()
    {
        foreach (var slot in inventory.items)
        {
            SlotUI uiSlot = Instantiate(slotPrefab, slotsParent);
            uiSlot.Setup(slot, this);
        }
    }

    public void ShowDetails(InventorySlot slot)
    {
        selectedSlot = slot;
        ItemData item = slot.item;

        detailIcon.sprite = item.Icon;
        detailIcon.enabled = true;
        detailName.text = item.ItemName;
        detailDescription.text = item.Description;


        useButton.gameObject.SetActive(item is UsableItemData);
    }


    public void UseSelectedItem()
    {
        if (selectedSlot == null) return;

        if (selectedSlot.item is UsableItemData usable)
        {
            usable.Use();
            
            if (usable.ConsumeOnUse)
            {
                inventory.RemoveItem(selectedSlot);
            }

            RefreshSlots();
            ClearDetails();
        }
    }

    void RefreshSlots()
    {
        foreach (Transform child in slotsParent)
        {
            child.GetComponent<SlotUI>().UpdateUI();
        }
    }

    void ClearDetails()
    {
        selectedSlot = null;
        detailIcon.enabled = false;
        detailName.text = "";
        detailDescription.text = "";
        useButton.gameObject.SetActive(false);
    }


    void ToggleInventory()
    {
        RefreshSlots();
        inventoryRoot.SetActive(!inventoryRoot.activeSelf);
        Debug.Log("Inventario toggle: " + inventoryRoot.activeSelf);
    }
}
