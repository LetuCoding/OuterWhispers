using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image fadePanel;   // Imagen negra fullscreen
    [SerializeField] private TMP_Text fadeText;     // Texto encima

    [Header("Timings")]
    [SerializeField] private float screenFadeDuration = 0.5f;
    [SerializeField] private float textFadeDuration = 0.35f;
    [SerializeField] private float textHoldTime = 1.2f;

    [Header("Texts")]
    [SerializeField] private List<string> texts;

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
    public void FadeToBlackAndPlayTexts()
    {
        StartCoroutine(FadeAndTextsRoutine());
    }

    private IEnumerator FadeAndTextsRoutine()
    {
        // 1) Pantalla a negro
        fadePanel.gameObject.SetActive(true);
        yield return FadePanelTo(1f, screenFadeDuration);

        // 2) Mostrar textos uno a uno con fade
        if (fadeText != null && texts != null && texts.Count > 0)
        {
            fadeText.gameObject.SetActive(true);

            foreach (var msg in texts)
            {
                fadeText.text = msg;

                // fade in texto
                yield return FadeTextTo(1f, textFadeDuration);

                // mantener
                yield return new WaitForSecondsRealtime(textHoldTime);

                // fade out texto
                yield return FadeTextTo(0f, textFadeDuration);

                // pequeña pausa opcional entre textos
                // yield return new WaitForSecondsRealtime(0.1f);
            }

            fadeText.gameObject.SetActive(false);
        }

        // 3) (Opcional) Volver desde negro
        // yield return FadePanelTo(0f, screenFadeDuration);
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
