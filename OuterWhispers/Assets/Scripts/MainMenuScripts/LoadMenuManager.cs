using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Zenject;

public class LoadMenuManager : MonoBehaviour
{
    private Button closeButton;
    private Button savedGameButton;
    private Button loadGameButton;

    [Header("Objeto raíz del menú de carga")]
    [SerializeField] private GameObject uiLoadMenu;
    [SerializeField] private GameObject uiMainMenu;

    private IAudioManager _audioManager;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource soundSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource rainSource;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip effect;
    [SerializeField] private AudioClip introMusic;

    private UIDocument uiDocument;
    private VisualElement root;

    [Inject]
    public void Construct(IAudioManager audioManager)
    {
        _audioManager = audioManager;
    }

    private void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
    }
    
    private void Update()
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame)
        {
            OnCloseClicked();
        }
    }
    
    private void OnEnable()
    {
        if (uiDocument == null)
            uiDocument = GetComponent<UIDocument>();

        if (uiDocument == null)
        {
            Debug.LogError("LoadMenuManager: no hay UIDocument en este GameObject.");
            return;
        }

        root = uiDocument.rootVisualElement;

        if (root == null)
        {
            Debug.LogError("LoadMenuManager: rootVisualElement es null.");
            return;
        }

        closeButton = root.Q<Button>("CloseButton");
        savedGameButton = root.Q<Button>("ButtonSavedGame");
        loadGameButton = root.Q<Button>("ButtonLoadGame");

        if (closeButton == null) Debug.LogError("No se encontró CloseButton");
        if (savedGameButton == null) Debug.LogError("No se encontró ButtonSavedGame");
        if (loadGameButton == null) Debug.LogError("No se encontró ButtonLoadGame");

        PrepararBoton(closeButton, OnCloseClicked);
        PrepararBoton(savedGameButton, OnNewGameClicked);
        PrepararBoton(loadGameButton, OnLoadGameClicked);

        StartCoroutine(SetInitialFocusNextFrame());
    }

    private void OnDisable()
    {
        LimpiarBoton(closeButton, OnCloseClicked);
        LimpiarBoton(savedGameButton, OnNewGameClicked);
        LimpiarBoton(loadGameButton, OnLoadGameClicked);
    }

    private void PrepararBoton(Button boton, System.Action onClick)
    {
        if (boton == null) return;

        boton.focusable = true;
        boton.clicked += onClick;

        boton.RegisterCallback<MouseEnterEvent>(OnHoverEnter);
        boton.RegisterCallback<MouseLeaveEvent>(OnHoverExit);

        boton.RegisterCallback<FocusInEvent>(OnFocusEnter);
        boton.RegisterCallback<FocusOutEvent>(OnFocusExit);
    }

    private void LimpiarBoton(Button boton, System.Action onClick)
    {
        if (boton == null) return;

        boton.clicked -= onClick;

        boton.UnregisterCallback<MouseEnterEvent>(OnHoverEnter);
        boton.UnregisterCallback<MouseLeaveEvent>(OnHoverExit);

        boton.UnregisterCallback<FocusInEvent>(OnFocusEnter);
        boton.UnregisterCallback<FocusOutEvent>(OnFocusExit);
    }

    private IEnumerator SetInitialFocusNextFrame()
    {
        yield return null;

        if (savedGameButton != null)
        {
            savedGameButton.Focus();
        }
    }

    public void SetInitialFocus()
    {
        StartCoroutine(SetInitialFocusNextFrame());
    }

    private void OnHoverEnter(MouseEnterEvent evt)
    {
        if (evt.currentTarget is Button boton)
        {
            ResaltarBoton(boton);
        }
    }

    private void OnHoverExit(MouseLeaveEvent evt)
    {
        if (evt.currentTarget is Button boton)
        {
            RestaurarBoton(boton);
        }
    }

    private void OnFocusEnter(FocusInEvent evt)
    {
        if (evt.currentTarget is Button boton)
        {
            ResaltarBoton(boton);
        }
    }

    private void OnFocusExit(FocusOutEvent evt)
    {
        if (evt.currentTarget is Button boton)
        {
            RestaurarBoton(boton);
        }
    }

    private void ResaltarBoton(Button boton)
    {
        boton.style.backgroundColor = new StyleColor(Color.grey);
    }

    private void RestaurarBoton(Button boton)
    {
        boton.style.backgroundColor = new StyleColor(new Color(0f, 0f, 0f, 0f));
    }

    private void OnCloseClicked()
    {
        if (_audioManager != null)
            _audioManager.PlaySFX(effect, soundSource, 1f);

        if (uiLoadMenu != null)
            uiLoadMenu.SetActive(false);
    }

    private void OnLoadGameClicked()
    {
        SceneBootData.ShouldLoadGame = true;

        if (_audioManager != null)
            _audioManager.PlaySFX(effect, soundSource, 1f);

        SceneManager.LoadScene("DemoLevel");
    }

    private void OnNewGameClicked()
    {
        SceneBootData.ShouldLoadGame = false;
        if (_audioManager != null)
            _audioManager.PlaySFX(effect, soundSource, 1f);

        if (uiLoadMenu != null)
            uiLoadMenu.SetActive(false);
        
        if (uiMainMenu != null)
            uiMainMenu.SetActive(false);
        
        ScreenFader fader = FindObjectOfType<ScreenFader>();
        if (fader != null)
        {
            fader.FadeToBlackAndPlayTexts();
        }
        else
        {
            Debug.LogWarning("No se encontró ScreenFader en la escena.");
        }

        if (_audioManager != null)
        {
            _audioManager.PlayMusic(introMusic, musicSource);
            _audioManager.StopRain(rainSource);
        }
    }
}