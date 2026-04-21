using System.Collections;
using System.Collections.Generic;
using InventoryScripts;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private Transform slotsParent;
    [SerializeField] private SlotUI slotPrefab;

    [Header("Details Panel")]
    [SerializeField] private Image detailIcon;
    [SerializeField] private TMP_Text detailName;
    [SerializeField] private TMP_Text detailDescription;
    [SerializeField] private Button useButton;

    [Header("Root")]
    [SerializeField] private GameObject inventoryRoot;

    private InventorySlot selectedSlot;
    private Player _player;

    private readonly List<SlotUI> uiSlots = new List<SlotUI>();

    private void Start()
    {
        BuildSlots();
        RefreshSlots();
        ClearDetails();
        if (inventoryRoot != null)
            inventoryRoot.SetActive(false);

        if (inventory != null && inventory.Owner != null)
            _player = inventory.Owner;

        if (useButton != null)
            useButton.onClick.AddListener(UseSelectedItem);

        if (inventory != null)
            inventory.EAddItem += OnInventoryChanged;
    }

    private void OnDestroy()
    {
        if (useButton != null)
            useButton.onClick.RemoveListener(UseSelectedItem);

        if (inventory != null)
            inventory.EAddItem -= OnInventoryChanged;
    }

    private void Update()
    {
        if (_player == null)
            return;

        // IMPORTANTE:
        // inventoryPressed debería ser algo tipo "pressed this frame".
        // Si es un bool mantenido, te abrirá/cerrará el inventario muchas veces.
        if (_player.inventoryPressed)
        {
            ToggleInventory();
            ClearDetails();
        }
    }

    private void OnInventoryChanged(Inventory inv)
    {
        RefreshSlots();

        if (inventoryRoot != null && inventoryRoot.activeSelf)
        {
            StartCoroutine(SelectFirstSlotNextFrame());
        }
    }

    public void BuildSlots()
    {
        uiSlots.Clear();

        if (inventory == null || slotsParent == null || slotPrefab == null)
            return;

        foreach (var slot in inventory.items)
        {
            SlotUI uiSlot = Instantiate(slotPrefab, slotsParent);
            uiSlot.Setup(slot, this);
            uiSlots.Add(uiSlot);
        }
    }

    public void LoadBuild()
    {
        foreach (Transform child in slotsParent)
        {
            Destroy(child.gameObject);
        }

        uiSlots.Clear();
        BuildSlots();
        RefreshSlots();
    }

    public void ShowDetails(InventorySlot slot)
    {
        selectedSlot = slot;

        if (slot == null || slot.item == null)
        {
            ClearDetails();
            return;
        }

        ItemData item = slot.item;

        if (detailIcon != null)
        {
            detailIcon.sprite = item.Icon;
            detailIcon.enabled = true;
        }

        if (detailName != null)
            detailName.text = item.ItemName;

        if (detailDescription != null)
            detailDescription.text = item.Description;

        if (useButton != null)
        {
            useButton.gameObject.SetActive(item is UsableItemData);
            EventSystem.current.SetSelectedGameObject(useButton.gameObject);
        }
    }

    public void UseSelectedItem()
    {
        if (selectedSlot == null || selectedSlot.item == null)
            return;

        if (selectedSlot.item is UsableItemData usable)
        {
            usable.Use(inventory.Owner._healthComponent);

            if (usable.ConsumeOnUse)
            {
                inventory.RemoveItem(selectedSlot);
            }

            RefreshSlots();
            ClearDetails();

            if (inventoryRoot != null && inventoryRoot.activeSelf)
                StartCoroutine(SelectFirstSlotNextFrame());
        }
    }

    public void RefreshSlots()
    {
        foreach (var uiSlot in uiSlots)
        {
            if (uiSlot != null)
                uiSlot.UpdateUI();
        }
    }

    private void ClearDetails()
    {
        selectedSlot = null;

        if (detailIcon != null)
        {
            detailIcon.enabled = false;
            detailIcon.sprite = null;
        }

        if (detailName != null)
            detailName.text = "";

        if (detailDescription != null)
            detailDescription.text = "";

        if (useButton != null)
            useButton.gameObject.SetActive(false);
    }

    private void ToggleInventory()
    {
        if (inventoryRoot == null)
            return;

        bool open = !inventoryRoot.activeSelf;
        inventoryRoot.SetActive(open);

        if (open)
        {
            _player.canMove = false;
            RefreshSlots();
            ClearDetails();
            StartCoroutine(SelectFirstSlotNextFrame());
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
            _player.canMove = true;
        }

        Debug.Log("Inventario toggle: " + inventoryRoot.activeSelf);
    }

    private IEnumerator SelectFirstSlotNextFrame()
    {
        yield return null;

        if (EventSystem.current == null)
            yield break;

        EventSystem.current.SetSelectedGameObject(null);

        // Primero intenta seleccionar el primer slot NO vacío
        foreach (var uiSlot in uiSlots)
        {
            if (uiSlot != null && !uiSlot.IsEmpty())
            {
                EventSystem.current.SetSelectedGameObject(uiSlot.gameObject);
                yield break;
            }
        }

        // Si todos están vacíos, selecciona el primero que exista
        if (uiSlots.Count > 0 && uiSlots[0] != null)
        {
            EventSystem.current.SetSelectedGameObject(uiSlots[0].gameObject);
        }
        else if (useButton != null && useButton.gameObject.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(useButton.gameObject);
        }
    }
}