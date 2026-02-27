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
    private IAudioManager _audioManager;
    
    [Header("Audio Sources")]
    [SerializeField] public AudioSource soundSource;
    
    [Header("SFX Clips")]
    public AudioClip effect;
    public AudioClip machineSound;
    
    [Inject]
    public void Construct(IAudioManager audioManager)
    {
        _audioManager = audioManager;
    }
    void Start()
    {
    }
    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
        CloseButton = root.Q<Button>("CloseButton");
        NoButton = root.Q<Button>("NoButton");
        YesButton = root.Q<Button>("YesButton");
        
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
        _audioManager.PlaySFX(effect, soundSource, 1f);
        UiOptions.SetActive(false);
    }
    private void OnYesClicked()
    {
        _audioManager.PlaySFX(machineSound, soundSource, 1f);
        UiOptions.SetActive(false);
    }

    private void OnNoClicked()
    {
        _audioManager.PlaySFX(machineSound, soundSource, 1f);
        UiOptions.SetActive(false);
    }


}
