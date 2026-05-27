using DG.Tweening;
using MoreMountains.Feedbacks;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class US_GameManager : MonoBehaviour
{
    [SerializeField] private RectTransform resourceButton;
    [SerializeField] private RectTransform toolButton;
    private bool resourceIsOpened = false;
    private bool toolIsOpened = false;

    [SerializeField] private RectTransform[] trainObjectsA;
    [SerializeField] private RectTransform[] trainObjectsB;

    private RectTransform[] trainObjects;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void MoveResourceButton()
    {
        if (toolIsOpened)
        {
            MoveToolButton();
        }
        trainObjects = trainObjectsA;
        resourceIsOpened = !resourceIsOpened;
        float targetYA = resourceIsOpened ? -1600f : -700f;

        resourceButton.DOAnchorPosX(targetYA, 0.2f)
            .SetEase(Ease.OutCubic);
        ToggleTrainEffect();
    }
    public void MoveToolButton()
    {
        if (resourceIsOpened) {
            MoveResourceButton();
        }
        trainObjects = trainObjectsB;
        toolIsOpened = !toolIsOpened;
        float targetYB = toolIsOpened ? -800f : -125f;

        toolButton.DOAnchorPosX(targetYB, 0.2f)
            .SetEase(Ease.OutCubic);
        ToggleTrainEffect();
    }

    private bool trainEffectActive = false;

    private Coroutine trainCoroutine;

    public void ToggleTrainEffect()
    {
        trainEffectActive = !trainEffectActive;

        if (trainEffectActive)
        {
            trainCoroutine = StartCoroutine(TrainEffectCoroutine());
        }
        else
        {
            StopCoroutine(trainCoroutine);

            StopAllTrainTweens();
        }
    }

    private IEnumerator TrainEffectCoroutine()
    {
        while (true)
        {
            for (int i = 0; i < trainObjects.Length; i++)
            {
                RectTransform rect = trainObjects[i];

                float randomHeight = UnityEngine.Random.Range(3f, 8f);

                float randomDuration = UnityEngine.Random.Range(0.03f, 0.08f);

                rect.DOKill();

                Tween tween = rect.DOAnchorPosY(
                    rect.anchoredPosition.y + randomHeight,
                    randomDuration
                )
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.OutQuad);

                // Wait until this bump finishes
                //yield return tween.WaitForCompletion();

                // Small delay between objects
                yield return new WaitForSeconds(
                    UnityEngine.Random.Range(0.03f, 0.08f)
                );
            }
            // RANDOM delay BEFORE next loop starts
            yield return new WaitForSeconds(
                UnityEngine.Random.Range(0.5f, 2f)
            );
        }
    }

    private void StopAllTrainTweens()
    {
        foreach (RectTransform rect in trainObjects)
        {
            rect.DOKill();

            rect.DOAnchorPosY(
                0f,
                0.15f
            );
        }
    }

    [Header("Fire UI")]
    [SerializeField] private Image fireFillImage;
    [SerializeField] private Image fireTopImage;
    [SerializeField] private UIAnimation001 fireDieAnimation;
    [SerializeField] private float minY = 0f;
    [SerializeField] private float maxY = 525f;
    private RectTransform fillChildRect;
    private bool fireOutTriggered = false;
    //[SerializeField] private float duration = 1f;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private ProgressBarController progressBar;
    //private int currentCount = 100;
    //private float timer = 0f;
    private void Awake()
    {
        //countText.text = currentCount.ToString();
        fillChildRect = fireFillImage.transform.GetChild(0)
            .GetComponent<RectTransform>();
    }
    private void OnEnable()
    {
        if (progressBar != null)
        {
            progressBar.BarUpdate += UpdateFireUI;
        }
    }

    private void OnDisable()
    {
        if (progressBar != null)
        {
            progressBar.BarUpdate -= UpdateFireUI;
        }
    }

    private void UpdateFireUI()
    {
        float currentValue = progressBar.CurrentValue;

        countText.text = currentValue.ToString("F0");

        FireDiesOut(currentValue);
    }
    //private void Update()
    //{
    //    // Timer accumulates time
    //    timer += Time.deltaTime;

    //    // When timer exceeds speed, decrease count
    //    if (timer >= (1f / duration) && currentCount > 0)
    //    {
    //        currentCount--; // Decrement count
    //        countText.text = currentCount.ToString(); // Update text
    //        FireDiesOut(currentCount); // Update UI
    //        timer = 0f;     // Reset timer

    //    }
    //}
    public void FireDiesOut(float value)
    {
        // Clamp value
        value = Mathf.Clamp(value, 0f, 100f);

        // Convert 0-100 into Y position
        // 100 = 0
        // 0 = 470

        float normalized = 1f - (value / progressBar.maxValue);

        float targetY = Mathf.Lerp(
            minY,
            maxY,
            normalized
        );

        // Apply Y position
        Vector2 anchoredPos = fillChildRect.anchoredPosition;

        anchoredPos.y = targetY;

        fillChildRect.anchoredPosition = anchoredPos;

        // Fire reached 0
        if (value <= 0f && !fireOutTriggered)
        {
            fireOutTriggered = true;
            if (fireTopImage != null)
            {
                fireTopImage.rectTransform
                    .DOScale(Vector3.one * 0.01f, 0.3f)
                    .SetEase(Ease.OutBack);
            }
            fireTopImage.gameObject.SetActive(false);
            countText.gameObject.SetActive(false);
            fireDieAnimation.gameObject.SetActive(true);
            fireDieAnimation.PlayAnimationOnce();
        }

        // Reset trigger if value restored
        if (value > 0f)
        {
            fireOutTriggered = false;
        }
    }
}
