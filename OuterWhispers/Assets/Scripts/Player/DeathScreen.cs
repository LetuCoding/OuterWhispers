using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image fadePanel;
    [SerializeField] private TMP_Text fadeText;
    [SerializeField] private GameObject deathOptions;

    [Header("Timings")]
    [SerializeField] private float screenFadeDuration = 4f;
    [SerializeField] private float textFadeDuration = 2f;

    [Header("Text")]
    [SerializeField] private string message = "THE NOTHING CALLS YOU";

    private void Awake()
    {
        SetPanelAlpha(0f);

        if (fadeText != null)
        {
            fadeText.gameObject.SetActive(false);
            SetTextAlpha(0f);
        }
    }

    // Llamas a esto desde donde quieras
    public void FadeToBlackAndShowMessage()
    {
        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        // 1️⃣ Fade pantalla a negro
        fadePanel.gameObject.SetActive(true);
        yield return FadePanelTo(1f, screenFadeDuration);

        // 2️⃣ Mostrar mensaje permanente
        if (fadeText != null)
        {
            fadeText.text = message;
            fadeText.gameObject.SetActive(true);

            // Fade IN del texto
            yield return FadeTextTo(1f, textFadeDuration);
        }
        deathOptions.SetActive(true);
        yield break;
    }

    private IEnumerator FadePanelTo(float targetAlpha, float duration)
    {
        float startAlpha = fadePanel.color.a;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(startAlpha, targetAlpha, t / duration);
            SetPanelAlpha(a);
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
            float a = Mathf.Lerp(startAlpha, targetAlpha, t / duration);
            SetTextAlpha(a);
            yield return null;
        }

        SetTextAlpha(targetAlpha);
    }

    private void SetPanelAlpha(float a)
    {
        if (fadePanel == null) return;
        Color c = fadePanel.color;
        c.a = a;
        fadePanel.color = c;
    }

    private void SetTextAlpha(float a)
    {
        if (fadeText == null) return;
        Color c = fadeText.color;
        c.a = a;
        fadeText.color = c;
    }
}
