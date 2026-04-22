using System.Collections;
using System.Collections.Generic;
using InventoryScripts;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private Transform slotsParent;
    [SerializeField] private SlotUI slotPrefab;

    [Header("Grid")]
    [SerializeField] private int columns = 4;

    [Header("Details Panel")]
    [SerializeField] private Image detailIcon;
    [SerializeField] private TMP_Text detailName;
    [SerializeField] private TMP_Text detailDescription;
    [SerializeField] private Button useButton;

    [Header("Root")]
    [SerializeField] private GameObject inventoryRoot;

    private InventorySlot selectedSlot;
    private readonly List<SlotUI> uiSlots = new List<SlotUI>();
    private Coroutine _selectRoutine;

    private void Start()
    {
        BuildSlots();
        RefreshSlots();
        ClearDetails();
        ConfigureNavigation();

        if (inventoryRoot != null)
            inventoryRoot.SetActive(false);

        if (useButton != null)
            useButton.onClick.AddListener(UseSelectedItem);

        if (inventory != null)
            inventory.EAddItem += OnInventoryChanged;
    }

    private void Update()
    {
        if (inventoryRoot == null || !inventoryRoot.activeInHierarchy)
            return;

        if (EventSystem.current == null)
            return;

        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

        // Botón círculo / B / Escape / Cancel
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TryDeselectUseButton(currentSelected);
            return;
        }

        if (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame)
        {
            TryDeselectUseButton(currentSelected);
        }
    }

    private void TryDeselectUseButton(GameObject currentSelected)
    {
        if (useButton == null || currentSelected != useButton.gameObject)
            return;

        SlotUI selectedUI = FindUISlotBySlot(selectedSlot);

        if (selectedUI != null && selectedUI.GetSelectable() != null)
        {
            EventSystem.current.SetSelectedGameObject(selectedUI.GetSelectable().gameObject);
        }
        else
        {
            StartSelectFirstSlot();
        }
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
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    private void OnInventoryChanged(Inventory inv)
    {
        RefreshSlots();
        ConfigureNavigation();

        if (inventoryRoot != null && inventoryRoot.activeSelf)
            StartSelectFirstSlot();
    }

    public void OnOpenedByManager()
    {
        RefreshSlots();
        ConfigureNavigation();
        ClearDetails();
        StartSelectFirstSlot();
    }

    public void BuildSlots()
    {
        uiSlots.Clear();

        if (inventory == null || slotsParent == null || slotPrefab == null)
            return;

        foreach (Transform child in slotsParent)
            Destroy(child.gameObject);

        foreach (var slot in inventory.items)
        {
            SlotUI uiSlot = Instantiate(slotPrefab, slotsParent);
            uiSlot.Setup(slot, this);
            uiSlots.Add(uiSlot);
        }
    }

    public void LoadBuild()
    {
        BuildSlots();
        RefreshSlots();
        ConfigureNavigation();
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
        }

        ConfigureNavigation();
    }

    public void UseSelectedItem()
    {
        if (selectedSlot == null || selectedSlot.item == null)
            return;

        if (selectedSlot.item is UsableItemData usable)
        {
            if (inventory != null && inventory.Owner != null && inventory.Owner._healthComponent != null)
                usable.Use(inventory.Owner._healthComponent);

            if (usable.ConsumeOnUse)
                inventory.RemoveItem(selectedSlot);

            RefreshSlots();
            ClearDetails();
            ConfigureNavigation();

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

    private void StartSelectFirstSlot()
    {
        if (_selectRoutine != null)
            StopCoroutine(_selectRoutine);

        _selectRoutine = StartCoroutine(SelectFirstSlotNextFrame());
    }

    private IEnumerator SelectFirstSlotNextFrame()
    {
        yield return null;
        yield return null;

        if (EventSystem.current == null)
            yield break;

        EventSystem.current.SetSelectedGameObject(null);

        Selectable first = GetFirstSelectable();
        if (first != null)
            EventSystem.current.SetSelectedGameObject(first.gameObject);
    }

    private Selectable GetFirstSelectable()
    {
        foreach (var uiSlot in uiSlots)
        {
            if (uiSlot != null && uiSlot.GetSelectable() != null)
                return uiSlot.GetSelectable();
        }

        return null;
    }

    private void ConfigureNavigation()
    {
        for (int i = 0; i < uiSlots.Count; i++)
        {
            SlotUI slotUI = uiSlots[i];
            if (slotUI == null)
                continue;

            Selectable current = slotUI.GetSelectable();
            if (current == null)
                continue;

            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.Explicit;

            nav.selectOnLeft = GetSelectableAt(i - 1, sameRowOnly: true, currentIndex: i);
            nav.selectOnRight = GetSelectableAt(i + 1, sameRowOnly: true, currentIndex: i);
            nav.selectOnUp = GetSelectableAt(i - columns, sameRowOnly: false, currentIndex: i);
            nav.selectOnDown = GetSelectableAt(i + columns, sameRowOnly: false, currentIndex: i);

            current.navigation = nav;
        }

        if (useButton != null)
        {
            Navigation buttonNav = new Navigation();
            buttonNav.mode = Navigation.Mode.Explicit;

            SlotUI selectedUI = FindUISlotBySlot(selectedSlot);
            if (selectedUI != null)
                buttonNav.selectOnUp = selectedUI.GetSelectable();

            useButton.navigation = buttonNav;
        }
    }

    private Selectable GetSelectableAt(int index, bool sameRowOnly, int currentIndex)
    {
        if (index < 0 || index >= uiSlots.Count)
            return null;

        if (sameRowOnly)
        {
            int currentRow = currentIndex / columns;
            int targetRow = index / columns;

            if (currentRow != targetRow)
                return null;
        }

        SlotUI slotUI = uiSlots[index];
        if (slotUI == null)
            return null;

        return slotUI.GetSelectable();
    }

    private SlotUI FindUISlotBySlot(InventorySlot slot)
    {
        if (slot == null)
            return null;

        foreach (var uiSlot in uiSlots)
        {
            if (uiSlot != null && uiSlot.GetSlot() == slot)
                return uiSlot;
        }

        return null;
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