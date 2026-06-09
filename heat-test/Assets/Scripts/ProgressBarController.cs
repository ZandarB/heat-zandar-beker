using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarController : MonoBehaviour
{
    [Header("Progress Bar Settings")]
    [SerializeField] public float maxValue;
    [SerializeField] public float startValue;

    [SerializeField] Image progressBar;
    [SerializeField] GameObject limitReachedIndicator;
    [SerializeField] TextMeshProUGUI valueText;

    public float currentValue;
    public event System.Action BarUpdate;
    public static ProgressBarController Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        currentValue = startValue;
        UpdateBar();
    }

    private void Update()
    {
        if (limitReachedIndicator != null)
            limitReachedIndicator.SetActive(currentValue >= maxValue);

        valueText.SetText($"{currentValue.ToString("F0")}/{maxValue}");
    }

    private void OnEnable()
    {
        BarUpdate += UpdateBar;
    }

    private void OnDisable()
    {
        BarUpdate -= UpdateBar;
    }

    // Add to the current value (e.g. elapsed time, XP gained, items collected)
    public int Increase(float amount)
    {
        float overflow = 0f;

        if (currentValue + amount > maxValue)
        {
            overflow = (currentValue + amount) - maxValue;
            currentValue = maxValue;
        }
        else
        {
            currentValue += amount;
        }

        BarUpdate?.Invoke();

        return Mathf.RoundToInt(overflow);
    }

    public void IncreaseMax(float amount)
    {
        maxValue += amount;
        currentValue = maxValue;

        BarUpdate?.Invoke();
    }

    // Subtract from the current value
    public void Decrease(float amount)
    {
        currentValue -= amount;
        currentValue = Mathf.Max(0, currentValue);
        BarUpdate?.Invoke();
    }

    // Directly set the current value (useful for timers driven by Time.deltaTime)
    public void SetValue(float value)
    {
        currentValue = Mathf.Clamp(value, 0, maxValue);
        BarUpdate?.Invoke();
    }

    // Reset back to the start value
    public void ResetBar()
    {
        currentValue = maxValue;
        BarUpdate?.Invoke();
    }

    private void UpdateBar()
    {
        progressBar.fillAmount = Mathf.Clamp01(currentValue / maxValue);
    }
}