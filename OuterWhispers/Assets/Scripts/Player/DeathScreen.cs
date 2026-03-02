using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class DeathScreen : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image fadePanel;
    [SerializeField] private TMP_Text fadeText;

    [Header("Buttons")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button returnButton;

    [Header("Timings")]
    [SerializeField] private float screenFadeDuration = 4f;
    [SerializeField] private float textFadeDuration = 2f;
    [SerializeField] private float buttonsDelay = 0.5f;

    [Header("Text")]
    [SerializeField] private string message = "THE NOTHING CALLS YOU";

    private void Awake()
    {
        // Panel invisible
        SetPanelAlpha(0f);

        // Texto oculto
        if (fadeText != null)
        {
            fadeText.gameObject.SetActive(false);
            SetTextAlpha(0f);
        }

        // ✅ Botones desactivados al iniciar
        if (restartButton != null)
            restartButton.gameObject.SetActive(false);

        if (returnButton != null)
            returnButton.gameObject.SetActive(false);
    }

    // Llamar desde donde quieras
    public void FadeToBlackAndShowMessage()
    {
        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        // 1️⃣ Fade a negro
        fadePanel.gameObject.SetActive(true);
        yield return FadePanelTo(1f, screenFadeDuration);

        // 2️⃣ Mostrar texto
        fadeText.text = message;
        fadeText.gameObject.SetActive(true);
        yield return FadeTextTo(1f, textFadeDuration);

        // 3️⃣ Pequeña pausa
        yield return new WaitForSecondsRealtime(buttonsDelay);

        // 4️⃣ Activar botones
        ShowButtons();
    }

    private void ShowButtons()
    {
        if (restartButton != null)
            restartButton.gameObject.SetActive(true);

        if (returnButton != null)
            returnButton.gameObject.SetActive(true);
    }

    private IEnumerator FadePanelTo(float targetAlpha, float duration)
    {
        float startAlpha = fadePanel.color.a;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            SetPanelAlpha(Mathf.Lerp(startAlpha, targetAlpha, t / duration));
            yield return null;
        }

        SetPanelAlpha(targetAlpha);
    }

    private IEnumerator FadeTextTo(float targetAlpha, float duration)
    {
        float startAlpha = fadeText.color.a;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            SetTextAlpha(Mathf.Lerp(startAlpha, targetAlpha, t / duration));
            yield return null;
        }

        SetTextAlpha(targetAlpha);
    }

    private void SetPanelAlpha(float a)
    {
        Color c = fadePanel.color;
        c.a = a;
        fadePanel.color = c;
    }

    private void SetTextAlpha(float a)
    {
        Color c = fadeText.color;
        c.a = a;
        fadeText.color = c;
    }

    public void RestartButton()
    {
        
        fadeText.gameObject.SetActive(false);
        fadePanel.gameObject.SetActive(false);
        returnButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        SceneManager.LoadScene("DemoLevel");
    }

    public void ReturnButton()
    {
        fadeText.gameObject.SetActive(false);
        fadePanel.gameObject.SetActive(false);
        returnButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        SceneManager.LoadScene("MainMenu");
    }
}
