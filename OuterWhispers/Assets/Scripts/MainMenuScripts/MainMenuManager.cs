using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Application = UnityEngine.Application;

public class MainMenuManager : MonoBehaviour
{
    private Button StartButton;
    private Button QuitButton;
    private Button OptionsButton;
    public GameObject UiOptions;
    public GameObject UiLoadMenu;

    void Start()
    {
        if (AudioManagerMenu.Instance != null)
            AudioManagerMenu.Instance.PlayMusic(AudioManagerMenu.Instance.menuMusic);
        if (AudioManagerMenu.Instance != null)
            AudioManagerMenu.Instance.PlayRain(AudioManagerMenu.Instance.rain);
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
        if (AudioManagerMenu.Instance != null)
            AudioManagerMenu.Instance.PlaySFX(AudioManagerMenu.Instance.clickSound);
        UiLoadMenu.SetActive(true);
    }

    private void OnQuitClicked()
    {
        if (AudioManagerMenu.Instance != null)
            AudioManagerMenu.Instance.PlaySFX(AudioManagerMenu.Instance.clickSound);
        Application.Quit();
    }

    private void OnOptionsClicked()
    {
        // Activa el UI Options
        if (AudioManagerMenu.Instance != null)
            AudioManagerMenu.Instance.PlaySFX(AudioManagerMenu.Instance.clickSound);
        UiOptions.SetActive(true);

    }
    void OnHoverEnterStart(MouseEnterEvent evt)
    {
        StartButton.style.backgroundColor = new StyleColor(Color.grey);
    }
    void OnHoverExitStart(MouseLeaveEvent evt)
    {
        StartButton.style.backgroundColor = new StyleColor(Color.black);
    }
    void OnHoverEnterQuit(MouseEnterEvent evt)
    {
        QuitButton.style.backgroundColor = new StyleColor(Color.grey);
    }
    void OnHoverExitQuit(MouseLeaveEvent evt)
    {
        QuitButton.style.backgroundColor = new StyleColor(Color.black);
    }
    void OnHoverEnterOptions(MouseEnterEvent evt)
    {
        OptionsButton.style.backgroundColor = new StyleColor(Color.grey);
    }
    void OnHoverExitOptions(MouseLeaveEvent evt)
    {
        OptionsButton.style.backgroundColor = new StyleColor(Color.black);
    }
}
