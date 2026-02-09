using UnityEngine;
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
    private IAudioSettings _audioSettings;
    private bool isPaused = false;
    
    [Inject]
    public void Construct(IAudioSettings audioSettings)
    {
        _audioSettings = audioSettings;
    }
    void Start()
    {
        _audioSettings.Play();
        UiOptions.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Debug.Log("ESC DETECTADO");

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
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
            CloseButton.clicked += OnCloseClicked;
            CloseButton.RegisterCallback<MouseEnterEvent>(OnHoverEnterClose);
            CloseButton.RegisterCallback<MouseLeaveEvent>(OnHoverExitClose);
        }

        if (sliderSound != null)
            sliderSound.RegisterValueChangedCallback(OnSliderSoundChanged);

        if (sliderMusic != null)
            sliderMusic.RegisterValueChangedCallback(OnSliderMusicChanged);
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
        float volume = evt.newValue / 100f;
        if (_audioSettings == null)
            return;
        _audioSettings.SetVolumeSFX(volume);
    }

    private void OnSliderMusicChanged(ChangeEvent<int> evt)
    {
        float volume = evt.newValue / 100f;

        if (_audioSettings == null)
        {
            return;
        }
        _audioSettings.SetVolumeMusic(volume);
    }

    private void OnCloseClicked()
    {
        if (AudioManagerMenu.Instance != null)
            AudioManagerMenu.Instance.PlaySFX(AudioManagerMenu.Instance.clickSound);
        UiOptions.SetActive(false);
    }
    public void PauseGame()
    {
        UiOptions.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        UiOptions.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
