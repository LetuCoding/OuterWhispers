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

    private bool _inputLock;
    private UIPanelType _currentPanel = UIPanelType.None;

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
        if (_inputLock)
            return;

        if (Keyboard.current == null)
            return;

        // =========================
        // INPUT OPCIONES
        // =========================
        bool escPressed = Keyboard.current.escapeKey.wasPressedThisFrame;
        bool startPressed = Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame;

        // =========================
        // INPUT INVENTARIO
        // =========================
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
        _inputLock = true;
        yield return null;
        _inputLock = false;
    }

    // =========================================================
    // INVENTARIO
    // =========================================================
    public void ToggleInventory()
    {
        if (inventoryPanel == null)
            return;

        if (_currentPanel == UIPanelType.Inventory)
            CloseInventory();
        else
            OpenInventory();
    }

    public void OpenInventory()
    {
        if (IsOptionsOpen())
            CloseOptions();

        inventoryPanel.SetActive(true);
        _currentPanel = UIPanelType.Inventory;

        PauseGame();

        if (inventoryUI != null)
            inventoryUI.OnOpenedByManager();
    }

    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        _currentPanel = UIPanelType.None;

        ResumeGameIfNeeded();
    }

    // =========================================================
    // OPCIONES
    // =========================================================
    public void ToggleOptions()
    {
        if (optionsPanel == null)
            return;

        if (_currentPanel == UIPanelType.Options)
            CloseOptions();
        else
            OpenOptions();
    }

    public void OpenOptions()
    {
        if (IsInventoryOpen())
            CloseInventory();

        optionsPanel.SetActive(true);
        _currentPanel = UIPanelType.Options;

        PauseGame();

        var options = optionsPanel.GetComponent<OptionsMenuManager>();
        if (options != null)
            options.OnOpenedByManager();
    }

    public void CloseOptions()
    {
        var options = optionsPanel.GetComponent<OptionsMenuManager>();
        if (options != null)
            options.OnClosedByManager();

        optionsPanel.SetActive(false);

        _currentPanel = UIPanelType.None;

        ResumeGameIfNeeded();
    }

    // =========================================================
    // ESTADO
    // =========================================================
    public bool IsInventoryOpen() => inventoryPanel != null && inventoryPanel.activeSelf;
    public bool IsOptionsOpen() => optionsPanel != null && optionsPanel.activeSelf;

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
        if (IsInventoryOpen() || IsOptionsOpen())
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