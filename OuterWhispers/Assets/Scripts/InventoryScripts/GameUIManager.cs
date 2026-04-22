using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject inventoryPanel; // Asigna aquí InventoryUI/Root
    [SerializeField] private GameObject optionsPanel;   // Asigna aquí UiOptions

    [Header("Input")]
    [SerializeField] private Key inventoryKey = Key.B;
    [SerializeField] private Key inventoryAltKey = Key.UpArrow;
    [SerializeField] private Key optionsKey = Key.Escape;

    [Header("Behaviour")]
    [SerializeField] private bool pauseGameWhenOpen = true;

    private bool _toggleLock;
    private UIPanelType _currentOpenPanel = UIPanelType.None;

    private enum UIPanelType
    {
        None,
        Inventory,
        Options
    }

    public bool IsAnyUIOpen => _currentOpenPanel != UIPanelType.None;
    public bool IsInventoryOpen => inventoryPanel != null && inventoryPanel.activeSelf;
    public bool IsOptionsOpen => optionsPanel != null && optionsPanel.activeSelf;

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
        if (_toggleLock)
            return;

        if (Keyboard.current == null)
            return;

        bool inventoryPressed =
            Keyboard.current[inventoryKey].wasPressedThisFrame ||
            Keyboard.current[inventoryAltKey].wasPressedThisFrame;

        bool optionsPressed =
            Keyboard.current[optionsKey].wasPressedThisFrame;

        if (inventoryPressed)
        {
            StartCoroutine(InputCooldown());
            ToggleInventory();
            return;
        }

        if (optionsPressed)
        {
            StartCoroutine(InputCooldown());
            ToggleOptions();
        }
    }

    private IEnumerator InputCooldown()
    {
        _toggleLock = true;
        yield return null;
        _toggleLock = false;
    }

    public void ToggleInventory()
    {
        if (inventoryPanel == null)
        {
            Debug.LogWarning("GameUIManager: inventoryPanel no asignado.");
            return;
        }

        if (IsInventoryOpen)
        {
            CloseInventory();
        }
        else
        {
            OpenInventory();
        }
    }

    public void ToggleOptions()
    {
        if (optionsPanel == null)
        {
            Debug.LogWarning("GameUIManager: optionsPanel no asignado.");
            return;
        }

        if (IsOptionsOpen)
        {
            CloseOptions();
        }
        else
        {
            OpenOptions();
        }
    }

    public void OpenInventory()
    {
        if (inventoryPanel == null)
            return;

        if (IsOptionsOpen)
            CloseOptions();

        inventoryPanel.SetActive(true);
        _currentOpenPanel = UIPanelType.Inventory;

        if (pauseGameWhenOpen)
            Time.timeScale = 0f;

        Debug.Log("Inventario abierto");
    }

    public void CloseInventory()
    {
        if (inventoryPanel == null)
            return;

        inventoryPanel.SetActive(false);

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        if (_currentOpenPanel == UIPanelType.Inventory)
            _currentOpenPanel = UIPanelType.None;

        ResumeGameIfNeeded();

        Debug.Log("Inventario cerrado");
    }

    public void OpenOptions()
    {
        if (optionsPanel == null)
            return;

        if (IsInventoryOpen)
            CloseInventory();

        optionsPanel.SetActive(true);
        _currentOpenPanel = UIPanelType.Options;

        if (pauseGameWhenOpen)
            Time.timeScale = 0f;

        OptionsMenuManager optionsMenu = optionsPanel.GetComponent<OptionsMenuManager>();
        if (optionsMenu != null)
            optionsMenu.OnOpenedByManager();

        Debug.Log("Opciones abiertas");
    }

    public void CloseOptions()
    {
        if (optionsPanel == null)
            return;

        OptionsMenuManager optionsMenu = optionsPanel.GetComponent<OptionsMenuManager>();
        if (optionsMenu != null)
            optionsMenu.OnClosedByManager();

        optionsPanel.SetActive(false);

        if (_currentOpenPanel == UIPanelType.Options)
            _currentOpenPanel = UIPanelType.None;

        ResumeGameIfNeeded();

        Debug.Log("Opciones cerradas");
    }

    public void CloseCurrentUI()
    {
        switch (_currentOpenPanel)
        {
            case UIPanelType.Inventory:
                CloseInventory();
                break;

            case UIPanelType.Options:
                CloseOptions();
                break;
        }
    }

    private void ResumeGameIfNeeded()
    {
        bool inventoryStillOpen = inventoryPanel != null && inventoryPanel.activeSelf;
        bool optionsStillOpen = optionsPanel != null && optionsPanel.activeSelf;

        if (!inventoryStillOpen && !optionsStillOpen)
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