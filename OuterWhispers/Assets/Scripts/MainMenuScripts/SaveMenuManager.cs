using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Zenject;

public class SaveMenuManager : MonoBehaviour
{
    private Button closeButton;
    private Button noButton;
    private Button yesButton;
    private Label saveText;

    [Header("Root")]
    [SerializeField] private GameObject uiOptions;

    [Header("Save Settings")]
    [SerializeField] private string requiredItemName = "Ink";

    [Header("Audio Sources")]
    [SerializeField] private AudioSource soundSource;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip effect;
    [SerializeField] private AudioClip machineSound;
    [SerializeField] private AudioClip typingSound;

    private IAudioManager audioManager;
    [Inject] private Player player;

    private OuterWhispersSaveSystem saveSystem;

    [Inject]
    public void Construct(IAudioManager injectedAudioManager)
    {
        audioManager = injectedAudioManager;
    }

    private void Awake()
    {
        if (uiOptions == null)
            uiOptions = gameObject;
    }

    private void Start()
    {
        saveSystem = new OuterWhispersSaveSystem();

        if (uiOptions != null)
            uiOptions.SetActive(false);
    }

    private void Update()
    {
        if (uiOptions == null || !uiOptions.activeInHierarchy)
            return;

        if (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame)
        {
            OnCloseClicked();
        }
    }

    public void Open()
    {
        if (GameUIManager.Instance != null)
            GameUIManager.Instance.SaveUIOpened();
        
        if (audioManager != null && machineSound != null && soundSource != null)
            audioManager.PlaySFX(machineSound, soundSource, 1f);

        if (uiOptions != null)
            uiOptions.SetActive(true);

        if (player != null)
            player.FreezePlayer();
        SetInitialFocus();
    }

    private void OnEnable()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
            return;
        VisualElement root = uiDocument.rootVisualElement;
        if (root == null)
            return;

        closeButton = root.Q<Button>("CloseButton");
        noButton = root.Q<Button>("NoButton");
        yesButton = root.Q<Button>("YesButton");
        saveText = root.Q<Label>("NoInkText");

        SetupButton(closeButton, OnCloseClicked, OnFocusEnterClose, OnFocusExitClose, OnHoverEnterClose, OnHoverExitClose);
        SetupButton(noButton, OnNoClicked, OnFocusEnterNo, OnFocusExitNo, OnHoverEnterNo, OnHoverExitNo);
        SetupButton(yesButton, OnYesClicked, OnFocusEnterYes, OnFocusExitYes, OnHoverEnterYes, OnHoverExitYes);
    }

    private void OnDisable()
    {
        TeardownButton(closeButton, OnCloseClicked, OnHoverEnterClose, OnHoverExitClose);
        TeardownButton(noButton, OnNoClicked, OnHoverEnterNo, OnHoverExitNo);
        TeardownButton(yesButton, OnYesClicked, OnHoverEnterYes, OnHoverExitYes);
    }

    private void SetupButton(
        Button button,
        System.Action clickAction,
        System.Action focusEnterAction,
        System.Action focusExitAction,
        EventCallback<MouseEnterEvent> hoverEnter,
        EventCallback<MouseLeaveEvent> hoverExit)
    {
        if (button == null)
            return;

        button.focusable = true;
        button.clicked += clickAction;

        button.RegisterCallback<MouseEnterEvent>(hoverEnter);
        button.RegisterCallback<MouseLeaveEvent>(hoverExit);
        button.RegisterCallback<FocusInEvent>(_ => focusEnterAction());
        button.RegisterCallback<FocusOutEvent>(_ => focusExitAction());
    }

    private void TeardownButton(
        Button button,
        System.Action clickAction,
        EventCallback<MouseEnterEvent> hoverEnter,
        EventCallback<MouseLeaveEvent> hoverExit)
    {
        if (button == null)
            return;

        button.clicked -= clickAction;
        button.UnregisterCallback<MouseEnterEvent>(hoverEnter);
        button.UnregisterCallback<MouseLeaveEvent>(hoverExit);
    }

    public void SetInitialFocus()
    {
        StartCoroutine(SetInitialFocusNextFrame());
    }

    private IEnumerator SetInitialFocusNextFrame()
    {
        yield return null;

        UIDocument uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
            yield break;

        VisualElement root = uiDocument.rootVisualElement;
        if (root == null)
            yield break;

        root.Focus();

        if (yesButton != null && yesButton.canGrabFocus)
        {
            yesButton.Focus();
            yesButton.focusable = true;
            yesButton.tabIndex = 0;
        }
        else if (noButton != null && noButton.canGrabFocus)
        {
            noButton.Focus();
            noButton.focusable = true;
            noButton.tabIndex = 1;
        }
        else if (closeButton != null && closeButton.canGrabFocus)
        {
            closeButton.Focus();
            closeButton.focusable = true;
            closeButton.tabIndex = 2;
        }
    }

    private void SetButtonHighlighted(Button button, bool highlighted)
    {
        if (button == null)
            return;

        button.style.backgroundColor = highlighted
            ? new StyleColor(Color.grey)
            : new StyleColor(new Color(0f, 0f, 0f, 0f));
    }

    private void OnFocusEnterNo() => SetButtonHighlighted(noButton, true);
    private void OnFocusExitNo() => SetButtonHighlighted(noButton, false);

    private void OnFocusEnterYes() => SetButtonHighlighted(yesButton, true);
    private void OnFocusExitYes() => SetButtonHighlighted(yesButton, false);

    private void OnFocusEnterClose() => SetButtonHighlighted(closeButton, true);
    private void OnFocusExitClose() => SetButtonHighlighted(closeButton, false);

    private void OnHoverEnterNo(MouseEnterEvent evt) => SetButtonHighlighted(noButton, true);
    private void OnHoverExitNo(MouseLeaveEvent evt) => SetButtonHighlighted(noButton, false);

    private void OnHoverEnterYes(MouseEnterEvent evt) => SetButtonHighlighted(yesButton, true);
    private void OnHoverExitYes(MouseLeaveEvent evt) => SetButtonHighlighted(yesButton, false);

    private void OnHoverEnterClose(MouseEnterEvent evt) => SetButtonHighlighted(closeButton, true);
    private void OnHoverExitClose(MouseLeaveEvent evt) => SetButtonHighlighted(closeButton, false);

    private void OnCloseClicked()
    {
        if (GameUIManager.Instance != null)
            GameUIManager.Instance.SaveUIClosed();
        
        player.UnfreezePlayer();
        if (saveText != null)
            saveText.text = " ";

        if (audioManager != null && effect != null && soundSource != null)
            audioManager.PlaySFX(effect, soundSource, 1f);

        if (uiOptions != null)
            uiOptions.SetActive(false);

        if (player != null)
            player.UnfreezePlayer();
    }

    private void OnNoClicked()
    {
        if (GameUIManager.Instance != null)
            GameUIManager.Instance.SaveUIClosed();
        
        if (saveText != null)
            saveText.text = " ";

        if (audioManager != null && machineSound != null && soundSource != null)
            audioManager.PlaySFX(machineSound, soundSource, 1f);

        if (uiOptions != null)
            uiOptions.SetActive(false);

        if (player != null)
            player.UnfreezePlayer();
    }

    private void OnYesClicked()
    {
        if (player == null || player.Inventory == null)
            return;

        if (!player.Inventory.CheckItemByName(requiredItemName))
        {
            if (saveText != null)
                saveText.text = "The typewriter needs ink";
            return;
        }

        player.Inventory.RemoveItemByName(requiredItemName);
        saveSystem.saveData(player, player.Inventory);

        if (audioManager != null && typingSound != null && soundSource != null)
            audioManager.PlaySFX(typingSound, soundSource, 1f);

        Debug.Log("Game Saved.");

        if (uiOptions != null)
            uiOptions.SetActive(false);

        player.UnfreezePlayer();
    }
}