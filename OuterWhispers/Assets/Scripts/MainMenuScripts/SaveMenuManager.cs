using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Zenject;

public class SaveMenuManager : MonoBehaviour
{
    private bool isOpen = false;
    private Button CloseButton;
    public GameObject UiOptions;
    private Button NoButton;
    private Button YesButton;
    private Label saveText;
    private IAudioManager _audioManager;
    [Inject] private Player _player;

    private OuterWhispersSaveSystem _saveSystem;

    [SerializeField] private string requiredItemName = "Ink";
    
    [Header("Audio Sources")]
    [SerializeField] public AudioSource soundSource;
    
    [Header("SFX Clips")]
    public AudioClip effect;
    public AudioClip machineSound;
    public AudioClip typingSound;
    
    [Inject]
    public void Construct(IAudioManager audioManager)
    {
        _audioManager = audioManager;
    }

    public void Open()
    {
        _audioManager.PlaySFX(machineSound, soundSource, 1f);
        UiOptions.SetActive(true);
        _player.FreezePlayer();
    }
    void Start()
    {
        UiOptions.SetActive(false);
        _saveSystem = new OuterWhispersSaveSystem();
    }
    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
        CloseButton = root.Q<Button>("CloseButton");
        NoButton = root.Q<Button>("NoButton");
        YesButton = root.Q<Button>("YesButton");
        saveText = root.Q<Label>("NoInkText");
        
        if (CloseButton != null) CloseButton.clicked += OnCloseClicked;
        if (NoButton != null) NoButton.clicked += OnNoClicked;
        if (YesButton != null) YesButton.clicked += OnYesClicked;
        NoButton.RegisterCallback<MouseEnterEvent>(OnHoverEnterNo);
        NoButton.RegisterCallback<MouseLeaveEvent>(OnHoverExitNo);
        YesButton.RegisterCallback<MouseEnterEvent>(OnHoverEnterYes);
        YesButton.RegisterCallback<MouseLeaveEvent>(OnHoverExitYes);
        CloseButton.RegisterCallback<MouseEnterEvent>(OnHoverEnterClose);
        CloseButton.RegisterCallback<MouseLeaveEvent>(OnHoverExitClose);

    }

    void OnHoverEnterNo(MouseEnterEvent evt)
    {
        NoButton.style.backgroundColor = new StyleColor(Color.grey);
    }
    void OnHoverExitNo(MouseLeaveEvent evt)
    {
        NoButton.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0));
    }
    void OnHoverEnterYes(MouseEnterEvent evt)
    {
        YesButton.style.backgroundColor = new StyleColor(Color.grey);
    }
    void OnHoverExitYes(MouseLeaveEvent evt)
    {
        YesButton.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0));
    }
    void OnHoverEnterClose(MouseEnterEvent evt)
    {
        CloseButton.style.backgroundColor = new StyleColor(Color.grey);
    }
    void OnHoverExitClose(MouseLeaveEvent evt)
    {
        CloseButton.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0));
    }
    
    private void OnCloseClicked()
    {
        saveText.text = " ";
        _audioManager.PlaySFX(effect, soundSource, 1f);
        UiOptions.SetActive(false);
        _player.UnfreezePlayer();
    }

    private void OnNoClicked()
    {
        saveText.text = " ";
        _audioManager.PlaySFX(machineSound, soundSource, 1f);
        UiOptions.SetActive(false);
        _player.UnfreezePlayer();
    }
    private void OnYesClicked()
    {
        _player.UnfreezePlayer();
        if (!_player.Inventory.CheckItemByName(requiredItemName))
        {
            saveText.text = "The typewriter needs ink";
            return;
        }
        _player.Inventory.RemoveItemByName(requiredItemName);
        _saveSystem.saveData(_player, _player.Inventory);
        _audioManager.PlaySFX(typingSound, soundSource, 1f);
        Debug.Log("Game Saved.");
        UiOptions.SetActive(false);
    }

}
