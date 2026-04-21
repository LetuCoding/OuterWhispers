using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;
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
        _audioManager.SetMusicVolume(0.5f, mixer);
        _audioManager.SetSoundVolume(0.5f, mixer);
        _audioManager.PlayRain(rain, rainSource, true);
        _audioManager.PlayMusic(menuMusic, musicSource);

        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        StartButton = root.Q<Button>("StartButton");
        QuitButton = root.Q<Button>("QuitButton");
        OptionsButton = root.Q<Button>("SettingsButton");
        StartCoroutine(ForzarFocus());
        if (StartButton != null)
        {
            StartButton.clicked += OnStartClicked;
            PrepararBoton(StartButton);
        }

        if (QuitButton != null)
        {
            QuitButton.clicked += OnQuitClicked;
            PrepararBoton(QuitButton);
        }

        if (OptionsButton != null)
        {
            OptionsButton.clicked += OnOptionsClicked;
            PrepararBoton(OptionsButton);
        }

        if (StartButton != null)
        {
            StartButton.Focus();
        }
    }

    private void PrepararBoton(Button boton)
    {
        boton.focusable = true;

        boton.RegisterCallback<MouseEnterEvent>(evt => ResaltarBoton(boton));
        boton.RegisterCallback<MouseLeaveEvent>(evt => RestaurarBoton(boton));

        boton.RegisterCallback<FocusInEvent>(evt => ResaltarBoton(boton));
        boton.RegisterCallback<FocusOutEvent>(evt => RestaurarBoton(boton));
    }

    private void ResaltarBoton(Button boton)
    {
        boton.style.backgroundColor = new StyleColor(Color.grey);
    }

    private void RestaurarBoton(Button boton)
    {
        boton.style.backgroundColor = new StyleColor(new Color(0f, 0f, 0f, 0f));
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
    IEnumerator ForzarFocus()
    {
        yield return null;
        var root = GetComponent<UIDocument>().rootVisualElement;
        root.Focus();
        StartButton?.Focus();
    }
}