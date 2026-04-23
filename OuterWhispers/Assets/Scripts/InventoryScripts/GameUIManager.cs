using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject optionsPanel;

    [Header("Refs")]
    [SerializeField] private InventoryUI inventoryUI;

    [Header("Settings")]
    [SerializeField] private bool pauseGameWhenOpen = true;

    private bool inputLock;
    private bool saveUIOpened;
    private UIPanelType currentPanel = UIPanelType.None;

    private enum UIPanelType
    {
        None,
        Inventory,
        Options
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (inputLock)
            return;

        if (saveUIOpened)
            return;

        if (Keyboard.current == null)
            return;

        bool escPressed = Keyboard.current.escapeKey.wasPressedThisFrame;
        bool startPressed = Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame;

        bool bPressed = Keyboard.current.bKey.wasPressedThisFrame;
        bool dpadUpPressed = Gamepad.current != null && Gamepad.current.dpad.up.wasPressedThisFrame;

        if (escPressed || startPressed)
        {
            StartCoroutine(InputCooldown());
            ToggleOptions();
            return;
        }

        if (bPressed || dpadUpPressed)
        {
            StartCoroutine(InputCooldown());
            ToggleInventory();
        }
    }

    private IEnumerator InputCooldown()
    {
        inputLock = true;
        yield return null;
        inputLock = false;
    }

    public void SaveUIOpened()
    {
        saveUIOpened = true;
        PauseGame();
    }

    public void SaveUIClosed()
    {
        saveUIOpened = false;
        ResumeGameIfNeeded();
    }

    public bool IsSaveUIOpened()
    {
        return saveUIOpened;
    }

    // =========================================================
    // INVENTARIO
    // =========================================================
    public void ToggleInventory()
    {
        if (inventoryPanel == null)
            return;

        if (saveUIOpened)
            return;

        if (currentPanel == UIPanelType.Inventory)
            CloseInventory();
        else
            OpenInventory();
    }

    public void OpenInventory()
    {
        if (saveUIOpened)
            return;

        if (IsOptionsOpen())
            CloseOptions();

        inventoryPanel.SetActive(true);
        currentPanel = UIPanelType.Inventory;

        PauseGame();

        if (inventoryUI != null)
            inventoryUI.OnOpenedByManager();
    }

    public void CloseInventory()
    {
        if (saveUIOpened)
            return;

        if (inventoryPanel == null)
            return;

        inventoryPanel.SetActive(false);

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        currentPanel = UIPanelType.None;

        ResumeGameIfNeeded();
    }

    // =========================================================
    // OPCIONES
    // =========================================================
    public void ToggleOptions()
    {
        if (optionsPanel == null)
            return;

        if (saveUIOpened)
            return;

        if (currentPanel == UIPanelType.Options)
            CloseOptions();
        else
            OpenOptions();
    }

    public void OpenOptions()
    {
        if (saveUIOpened)
            return;

        if (IsInventoryOpen())
            CloseInventory();

        optionsPanel.SetActive(true);
        currentPanel = UIPanelType.Options;

        PauseGame();

        var options = optionsPanel.GetComponent<OptionsMenuManager>();
        if (options != null)
            options.OnOpenedByManager();
    }

    public void CloseOptions()
    {
        if (saveUIOpened)
            return;

        if (optionsPanel == null)
            return;

        var options = optionsPanel.GetComponent<OptionsMenuManager>();
        if (options != null)
            options.OnClosedByManager();

        optionsPanel.SetActive(false);

        currentPanel = UIPanelType.None;

        ResumeGameIfNeeded();
    }

    // =========================================================
    // ESTADO
    // =========================================================
    public bool IsInventoryOpen()
    {
        return inventoryPanel != null && inventoryPanel.activeSelf;
    }

    public bool IsOptionsOpen()
    {
        return optionsPanel != null && optionsPanel.activeSelf;
    }

    public bool IsAnyUIOpen()
    {
        return saveUIOpened || IsInventoryOpen() || IsOptionsOpen();
    }

    // =========================================================
    // PAUSA GLOBAL
    // =========================================================
    private void PauseGame()
    {
        if (!pauseGameWhenOpen)
            return;

        Time.timeScale = 0f;
    }

    private void ResumeGameIfNeeded()
    {
        if (saveUIOpened || IsInventoryOpen() || IsOptionsOpen())
            return;

        Time.timeScale = 1f;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Time.timeScale = 1f;
            Instance = null;
        }
    }
}