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

    private Coroutine _selectRoutine;
    private bool _toggleLock;

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

    private void OnDisable()
    {
        if (_player != null)
            _player.UnPauseMenu();
    }

    private void Update()
    {
        if (_player == null)
            return;

        // Esto asume que inventoryPressed viene como pulsación de un solo frame.
        // Si en Player no está bien reseteado, el inventario toggleará varias veces.
        if (_player.inventoryPressed && !_toggleLock)
        {
            StartCoroutine(ToggleInventoryCooldown());
            ToggleInventory();
        }
    }

    private IEnumerator ToggleInventoryCooldown()
    {
        _toggleLock = true;
        yield return null;
        _toggleLock = false;
    }

    private void OnInventoryChanged(Inventory inv)
    {
        RefreshSlots();

        if (inventoryRoot != null && inventoryRoot.activeSelf)
        {
            StartSelectFirstSlot();
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
            bool canUse = item is UsableItemData;
            useButton.gameObject.SetActive(canUse);

            // IMPORTANTE:
            // NO seleccionar aquí el botón.
            // ShowDetails suele ser llamado desde OnSelect del slot,
            // y cambiar la selección aquí provoca el conflicto con EventSystem.
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
                StartSelectFirstSlot();
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
            RefreshSlots();
            ClearDetails();
            StartSelectFirstSlot();
        }
        else
        {
            if (EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(null);
        }

        Debug.Log("Inventario toggle: " + inventoryRoot.activeSelf);
    }

    private void StartSelectFirstSlot()
    {
        if (_selectRoutine != null)
            StopCoroutine(_selectRoutine);

        _selectRoutine = StartCoroutine(SelectFirstSlotNextFrame());
    }

    private IEnumerator SelectFirstSlotNextFrame()
    {
        // Esperar más de un frame ayuda a evitar conflictos con el EventSystem
        yield return null;
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

    public void FocusUseButton()
    {
        if (useButton == null || !useButton.gameObject.activeInHierarchy)
            return;

        StartCoroutine(FocusUseButtonNextFrame());
    }

    private IEnumerator FocusUseButtonNextFrame()
    {
        yield return null;
        yield return null;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(useButton.gameObject);
    }
}