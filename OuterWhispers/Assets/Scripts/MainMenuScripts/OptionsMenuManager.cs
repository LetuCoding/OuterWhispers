using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Zenject;
using Cursor = UnityEngine.Cursor;

public class OptionsMenuManager : MonoBehaviour
{
    private bool isOpen = false;
    private Button CloseButton;
    public GameObject UiOptions;
    private SliderInt sliderSound;
    private SliderInt sliderMusic;
    private IAudioManager _audioManager;
    private bool _isPaused;
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
        UiOptions.SetActive(false);
    }
    void Update()
    {

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
            CloseButton.clicked += OnCloseClicked;
            CloseButton.RegisterCallback<MouseEnterEvent>(OnHoverEnterClose);
            CloseButton.RegisterCallback<MouseLeaveEvent>(OnHoverExitClose);
        }
        

        if (sliderSound != null)
        {
            sliderSound.RegisterValueChangedCallback(OnSliderSoundChanged);
            sliderSound.SetValueWithoutNotify(savedVolume);
        }

        if (sliderMusic != null)
        {
            sliderMusic.RegisterValueChangedCallback(OnSliderMusicChanged);
            sliderMusic.SetValueWithoutNotify(savedMusic);
        }
    }

    void OnHoverEnterClose(MouseEnterEvent evt)
    {
        CloseButton.style.backgroundColor = new StyleColor(Color.grey);
    }
    void OnHoverExitClose(MouseLeaveEvent evt)
    {
        CloseButton.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0));
    }
    private void OnDisable()
    {
        if (sliderSound != null)
            sliderSound.UnregisterValueChangedCallback(OnSliderSoundChanged);

        if (sliderMusic != null)
            sliderMusic.UnregisterValueChangedCallback(OnSliderMusicChanged);
    }

    private void OnSliderSoundChanged(ChangeEvent<int> evt)
    {
        savedVolume = evt.newValue;
        float volume = evt.newValue / 100f;
        if (_audioManager != null)
        {
            _audioManager.SetSoundVolume(volume,mixer);
        }
    }

    private void OnSliderMusicChanged(ChangeEvent<int> evt)
    {
        savedMusic = evt.newValue;
        float volume = evt.newValue / 100f;

        if (_audioManager != null)
        {
            _audioManager.SetMusicVolume(volume,mixer);
        }
    }

    private void OnCloseClicked()
    {
        _audioManager.PlaySFX(effect, soundSource, 1f);
        UiOptions.SetActive(false);
    }
}
