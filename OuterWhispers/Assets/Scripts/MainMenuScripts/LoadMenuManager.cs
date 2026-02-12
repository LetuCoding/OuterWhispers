using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Zenject;

public class LoadMenuManager : MonoBehaviour
{
    private bool isOpen = false;
    private Button CloseButton;
    public GameObject UiOptions;
    private Button ButtonSavedGameButton;
    private Button ButtonLoadGameButton;
    private IAudioManager _audioManager;
    
    [Header("Audio Sources")]
    [SerializeField] public AudioSource soundSource;
    [SerializeField] public AudioSource musicSource;
    [SerializeField] public AudioSource rainSource;
    
    [Header("SFX Clips")]
    public AudioClip effect;
    public AudioClip introMusic;
    
    [Header("Audio Mixer")]
    public AudioMixer mixer;
    
    [Inject]
    public void Construct(IAudioManager audioManager)
    {
        _audioManager = audioManager;
    }
    void Start()
    {
        UiOptions.SetActive(false);
    }
    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
        CloseButton = root.Q<Button>("CloseButton");
        ButtonSavedGameButton = root.Q<Button>("ButtonSavedGame");
        ButtonLoadGameButton = root.Q<Button>("ButtonNewGame");
        
        if (CloseButton != null) CloseButton.clicked += OnCloseClicked;
        if (ButtonSavedGameButton != null) ButtonSavedGameButton.clicked += OnNewGameClicked;
        if (ButtonLoadGameButton != null) ButtonLoadGameButton.clicked += OnLoadClicked;
        ButtonSavedGameButton.RegisterCallback<MouseEnterEvent>(OnHoverEnterSaved);
        ButtonSavedGameButton.RegisterCallback<MouseLeaveEvent>(OnHoverExitSaved);
        ButtonLoadGameButton.RegisterCallback<MouseEnterEvent>(OnHoverEnterLoad);
        ButtonLoadGameButton.RegisterCallback<MouseLeaveEvent>(OnHoverExitLoad);
        CloseButton.RegisterCallback<MouseEnterEvent>(OnHoverEnterClose);
        CloseButton.RegisterCallback<MouseLeaveEvent>(OnHoverExitClose);

    }

    void OnHoverEnterSaved(MouseEnterEvent evt)
    {
        ButtonSavedGameButton.style.backgroundColor = new StyleColor(Color.grey);
    }
    void OnHoverExitSaved(MouseLeaveEvent evt)
    {
        ButtonSavedGameButton.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0));
    }
    void OnHoverEnterLoad(MouseEnterEvent evt)
    {
        ButtonLoadGameButton.style.backgroundColor = new StyleColor(Color.grey);
    }
    void OnHoverExitLoad(MouseLeaveEvent evt)
    {
        ButtonLoadGameButton.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0));
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
    private void OnLoadClicked()
    {
        _audioManager.PlaySFX(effect, soundSource, 1f);
        UiOptions.SetActive(false);
        FindObjectOfType<ScreenFader>().FadeToBlackAndPlayTexts();
        _audioManager.PlayMusic(introMusic, musicSource);
        _audioManager.StopRain(rainSource);
    }

    private void OnNewGameClicked()
    {
        _audioManager.PlaySFX(effect, soundSource, 1f);
    }


}
