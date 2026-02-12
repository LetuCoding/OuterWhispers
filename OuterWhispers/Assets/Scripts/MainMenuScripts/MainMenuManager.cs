using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Zenject;
using Application = UnityEngine.Application;

public class MainMenuManager : MonoBehaviour
{
    private Button StartButton;
    private Button QuitButton;
    private Button OptionsButton;
    public GameObject UiOptions;
    public GameObject UiLoadMenu;
    [Header("Audio Sources")]
    [SerializeField] public AudioSource musicSource;
    [SerializeField] public AudioSource soundSource;
    [SerializeField] public AudioSource rainSource;
    
    [Header("SFX Clips")]
    public AudioClip rain;
    public AudioClip menuMusic;
    public AudioClip menuSound;
    
    [Header("Audio Mixer")]
    public AudioMixer mixer;

    private IAudioManager _audioManager;
    
    [Inject]
    public void Construct(IAudioManager audioManager)
    {
        _audioManager = audioManager;
    }
    void Start()
    {
        _audioManager.SetMusicVolume(0.5f,mixer);
        _audioManager.SetSoundVolume(0.5f,mixer);
        _audioManager.PlayRain(rain, rainSource, true);
        _audioManager.PlayMusic(menuMusic, musicSource);
    }
    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
        StartButton = root.Q<Button>("StartButton");
        QuitButton = root.Q<Button>("QuitButton");
        OptionsButton = root.Q<Button>("SettingsButton");

        if (StartButton != null) StartButton.clicked += OnStartClicked;
        if (QuitButton != null) QuitButton.clicked += OnQuitClicked;
        if (OptionsButton != null) OptionsButton.clicked += OnOptionsClicked;
        StartButton.RegisterCallback<MouseEnterEvent>(OnHoverEnterStart);
        StartButton.RegisterCallback<MouseLeaveEvent>(OnHoverExitStart);
        QuitButton.RegisterCallback<MouseEnterEvent>(OnHoverEnterQuit);
        QuitButton.RegisterCallback<MouseLeaveEvent>(OnHoverExitQuit);
        OptionsButton.RegisterCallback<MouseEnterEvent>(OnHoverEnterOptions);
        OptionsButton.RegisterCallback<MouseLeaveEvent>(OnHoverExitOptions);

    }

    private void OnStartClicked()
    {
        _audioManager.PlaySFX(menuSound, soundSource, 1f);
        UiLoadMenu.SetActive(true);
    }

    private void OnQuitClicked()
    {
        _audioManager.PlaySFX(menuSound, soundSource, 1f);
        Application.Quit();
    }

    private void OnOptionsClicked()
    {
        _audioManager.PlaySFX(menuSound, soundSource, 1f);
        UiOptions.SetActive(true);

    }
    void OnHoverEnterStart(MouseEnterEvent evt)
    {
        StartButton.style.backgroundColor = new StyleColor(Color.grey);
    }
    void OnHoverExitStart(MouseLeaveEvent evt)
    {
        StartButton.style.backgroundColor = new StyleColor(new Color(0f, 0f, 0f, 0f));
    }
    void OnHoverEnterQuit(MouseEnterEvent evt)
    {
        QuitButton.style.backgroundColor = new StyleColor(Color.grey);
    }
    void OnHoverExitQuit(MouseLeaveEvent evt)
    {
        QuitButton.style.backgroundColor = new StyleColor(new Color(0f, 0f, 0f, 0f));
    }
    void OnHoverEnterOptions(MouseEnterEvent evt)
    {
        OptionsButton.style.backgroundColor = new StyleColor(Color.grey);
    }
    void OnHoverExitOptions(MouseLeaveEvent evt)
    {
        OptionsButton.style.backgroundColor = new StyleColor(new Color(0f, 0f, 0f, 0f));
    }
}
