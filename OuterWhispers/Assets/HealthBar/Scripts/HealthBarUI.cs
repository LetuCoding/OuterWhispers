using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private RectTransform healthFillRect;
    [SerializeField] private Image healthFillImage;
    [SerializeField] private HealthComponent playerHealthComponent;
    [SerializeField] private RectTransform containerRect; 
    
    [SerializeField] private float updateSpeed = 10f;
    [SerializeField] private Gradient colorGradient;
    
    [Range(0f, 1f)]
    [SerializeField] private float flashThreshold = 0.3f;
    [SerializeField] private float flashSpeed = 5f;

    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeStrength = 5f;    
    
    private float targetScale = 1f;
    private float currentShakeTime = 0f;
    private Vector2 originalPosition;
    private float lastKnownHealth;

    private void Awake()
    {
        if (containerRect == null) containerRect = GetComponent<RectTransform>();
        originalPosition = containerRect.anchoredPosition;
    }

    private void OnEnable()
    {
        if (playerHealthComponent != null)
        {
            playerHealthComponent.OnHealthChanged += UpdateHealthBar;
            UpdateHealthBar(100, 100, true);
        }
    }

    private void OnDisable()
    {
        if (playerHealthComponent != null)
            playerHealthComponent.OnHealthChanged -= UpdateHealthBar;
    }

    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        UpdateHealthBar(currentHealth, maxHealth, false);
    }

    private void UpdateHealthBar(float currentHealth, float maxHealth, bool isInit)
    {
        if (maxHealth == 0) maxHealth = 1;
        if (!isInit && currentHealth < lastKnownHealth)
        {
            TriggerShake();
        }

        lastKnownHealth = currentHealth;
        targetScale = currentHealth / maxHealth;
    }

    private void TriggerShake()
    {
        currentShakeTime = shakeDuration;
    }

    private void Update()
    {
        if (healthFillRect.localScale.x != targetScale)
        {
            Vector3 newScale = healthFillRect.localScale;
            newScale.x = Mathf.Lerp(healthFillRect.localScale.x, targetScale, Time.deltaTime * updateSpeed);
            healthFillRect.localScale = newScale;
        }
        
        if (healthFillImage != null)
        {
            Color currentColor = colorGradient.Evaluate(healthFillRect.localScale.x);

            if (healthFillRect.localScale.x <= flashThreshold)
            {
                float flash = 0.4f + Mathf.PingPong(Time.time * flashSpeed, 0.6f);
                currentColor.a = flash;
            }
            else
            {
                currentColor.a = 1f;
            }
            healthFillImage.color = currentColor;
        }
        
        if (currentShakeTime > 0)
        {
            currentShakeTime -= Time.deltaTime;
            Vector2 randomOffset = Random.insideUnitCircle * shakeStrength;
            containerRect.anchoredPosition = originalPosition + randomOffset;
        }
        else
        {
            if (containerRect.anchoredPosition != originalPosition)
            {
                containerRect.anchoredPosition = originalPosition;
            }
        }
    }
}