using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Zenject;

public class OptionsMenuManager : MonoBehaviour
{
    private Button CloseButton;
    public GameObject UiOptions;
    public Player Player;
    private SliderInt sliderSound;
    private SliderInt sliderMusic;
    private IAudioManager _audioManager;
    private int savedVolume = 50;
    private int savedMusic = 50;

    [Header("Audio Sources")]
    [SerializeField] public AudioSource soundSource;

    [Header("SFX Clips")]
    public AudioClip effect;

    [Header("Audio Mixer")]
    public AudioMixer mixer;

    [Inject]
    public void Construct(IAudioManager audioManager)
    {
        _audioManager = audioManager;
    }

    void Start()
    {
        if (UiOptions != null)
            UiOptions.SetActive(false);
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame)
        {
            RequestClose();
        }
    }

    void OnEnable()
    {
        var uiDocument = UiOptions != null ? UiOptions.GetComponent<UIDocument>() : GetComponent<UIDocument>();
        if (uiDocument == null) return;

        var root = uiDocument.rootVisualElement;

        CloseButton = root.Q<Button>("CloseButton");
        sliderSound = root.Q<SliderInt>("SliderSound");
        sliderMusic = root.Q<SliderInt>("SliderMusic");

        if (CloseButton != null)
        {
            CloseButton.clicked += RequestClose;
            CloseButton.focusable = true;
            CloseButton.RegisterCallback<MouseEnterEvent>(OnHoverEnterClose);
            CloseButton.RegisterCallback<MouseLeaveEvent>(OnHoverExitClose);
            CloseButton.RegisterCallback<FocusInEvent>(_ => OnFocusEnterClose());
            CloseButton.RegisterCallback<FocusOutEvent>(_ => OnFocusExitClose());
        }

        if (sliderSound != null)
        {
            sliderSound.focusable = true;
            sliderSound.RegisterValueChangedCallback(OnSliderSoundChanged);
            sliderSound.SetValueWithoutNotify(savedVolume);
        }

        if (sliderMusic != null)
        {
            sliderMusic.focusable = true;
            sliderMusic.RegisterValueChangedCallback(OnSliderMusicChanged);
            sliderMusic.SetValueWithoutNotify(savedMusic);
        }
    }

    private void OnDisable()
    {
        if (Player != null)
            Player.UnPauseMenu();

        if (CloseButton != null)
        {
            CloseButton.clicked -= RequestClose;
            CloseButton.UnregisterCallback<MouseEnterEvent>(OnHoverEnterClose);
            CloseButton.UnregisterCallback<MouseLeaveEvent>(OnHoverExitClose);
        }

        if (sliderSound != null)
            sliderSound.UnregisterValueChangedCallback(OnSliderSoundChanged);

        if (sliderMusic != null)
            sliderMusic.UnregisterValueChangedCallback(OnSliderMusicChanged);
    }

    public void OnOpenedByManager()
    {
        SetInitialFocus();
    }

    public void OnClosedByManager()
    {
    }

    private void RequestClose()
    {
        if (_audioManager != null && effect != null && soundSource != null)
            _audioManager.PlaySFX(effect, soundSource, 1f);

        if (GameUIManager.Instance != null)
            GameUIManager.Instance.CloseOptions();
        else if (UiOptions != null)
            UiOptions.SetActive(false);
    }

    public void SetInitialFocus()
    {
        StartCoroutine(SetInitialFocusNextFrame());
    }

    private IEnumerator SetInitialFocusNextFrame()
    {
        yield return null;

        var uiDocument = UiOptions != null ? UiOptions.GetComponent<UIDocument>() : GetComponent<UIDocument>();
        if (uiDocument == null) yield break;

        var root = uiDocument.rootVisualElement;
        if (root == null) yield break;

        root.Focus();

        if (sliderSound != null && sliderSound.canGrabFocus)
        {
            sliderSound.Focus();
        }
        else if (sliderMusic != null && sliderMusic.canGrabFocus)
        {
            sliderMusic.Focus();
        }
        else if (CloseButton != null && CloseButton.canGrabFocus)
        {
            CloseButton.Focus();
        }
    }

    void OnHoverEnterClose(MouseEnterEvent evt)
    {
        if (CloseButton != null)
            CloseButton.style.backgroundColor = new StyleColor(Color.grey);
    }

    void OnHoverExitClose(MouseLeaveEvent evt)
    {
        if (CloseButton != null)
            CloseButton.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0));
    }

    void OnFocusEnterClose()
    {
        if (CloseButton != null)
            CloseButton.style.backgroundColor = new StyleColor(Color.grey);
    }

    void OnFocusExitClose()
    {
        if (CloseButton != null)
            CloseButton.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0));
    }

    private void OnSliderSoundChanged(ChangeEvent<int> evt)
    {
        savedVolume = evt.newValue;
        float volume = evt.newValue / 100f;

        if (_audioManager != null)
            _audioManager.SetSoundVolume(volume, mixer);
    }

    private void OnSliderMusicChanged(ChangeEvent<int> evt)
    {
        savedMusic = evt.newValue;
        float volume = evt.newValue / 100f;

        if (_audioManager != null)
            _audioManager.SetMusicVolume(volume, mixer);
    }
}