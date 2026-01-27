using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class OptionsMenuManager : MonoBehaviour
{
    private bool isOpen = false;
    private Button CloseButton;
    public GameObject UiOptions;
    private SliderInt sliderSound;
    private SliderInt sliderMusic;
    void Start()
    {
        UiOptions.SetActive(false);
    }
    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
        CloseButton = root.Q<Button>("CloseButton");
        sliderSound = root.Q<SliderInt>("SliderSound");
        sliderMusic = root.Q<SliderInt>("SliderMusic");
        if (CloseButton != null) CloseButton.clicked += OnCloseClicked;
        
        if (sliderSound != null)
        {
            sliderSound.RegisterValueChangedCallback(OnSliderSoundChanged);
        }

        if (sliderMusic != null)
        {
            sliderMusic.RegisterValueChangedCallback(OnSliderMusicChanged);
        }

        CloseButton.RegisterCallback<MouseEnterEvent>(OnHoverEnterClose);
        CloseButton.RegisterCallback<MouseLeaveEvent>(OnHoverExitClose);

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
        float vol = evt.newValue / 100f;
    }

    private void OnSliderMusicChanged(ChangeEvent<int> evt)
    {
        float vol = evt.newValue / 100f;
    }
    private void OnCloseClicked()
    {
        if (AudioManagerMenu.Instance != null)
            AudioManagerMenu.Instance.PlaySFX(AudioManagerMenu.Instance.clickSound);
        UiOptions.SetActive(false);

    }
}
