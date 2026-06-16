using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;

public class FishingMinigame_Input : MonoBehaviour
{
    public enum State { Idle, WaitingForBite, Running, Result }

    [Header("References")]
    public RectTransform trackArea;
    public RectTransform marker;
    public RectTransform successZone;
    public TMP_Text resultText;

    [Header("Gameplay")]
    public float speed = 1.5f;
    public Vector2 biteWaitRange = new Vector2(0.75f, 1.75f);
    public bool randomizeZone = true;
    public Vector2 zoneSizeRange = new Vector2(0.18f, 0.32f);
    public Vector2 zoneCenterClamp = new Vector2(0.15f, 0.85f);
    public bool autoStartOnEnable = true;

    [Header("Input (New Input System)")]
    [Tooltip("Assign an InputActionReference to a Button-type action (e.g., Gameplay/Stop).")]
    public InputActionReference stopAction;

    [Header("Events")]
    public UnityEvent onCatch;
    public UnityEvent onMiss;

    private State state = State.Idle;
    private float t;
    private int dir = 1;
    private float biteTimer;

    void OnEnable()
    {
        if (stopAction != null && stopAction.action != null)
        {
            stopAction.action.performed += OnStopPerformed;
            stopAction.action.Enable();
        }

        if (autoStartOnEnable) StartFishing();
    }

    void OnDisable()
    {
        if (stopAction != null && stopAction.action != null)
        {
            stopAction.action.performed -= OnStopPerformed;
            stopAction.action.Disable();
        }
    }

    void Update()
    {
        switch (state)
        {
            case State.WaitingForBite:
                biteTimer -= Time.deltaTime;
                if (biteTimer <= 0f)
                {
                    state = State.Running;
                    if (resultText) resultText.text = "";
                }
                break;

            case State.Running:
                UpdateMarker();
                break;
        }
    }

    private void OnStopPerformed(InputAction.CallbackContext ctx)
    {
        if (state == State.Running)
            Evaluate();
    }

    public void StartFishing()
    {
        if (!ValidateRefs()) return;
        if (resultText) resultText.text = "";

        if (randomizeZone) RandomizeZone();

        t = Random.Range(0.05f, 0.95f);
        dir = Random.value < 0.5f ? 1 : -1;
        ApplyMarkerPosition();

        biteTimer = Random.Range(biteWaitRange.x, biteWaitRange.y);
        state = State.WaitingForBite;
    }

    public void CancelFishing()
    {
        state = State.Idle;
        if (resultText) resultText.text = "";
    }

    private void UpdateMarker()
    {
        t += dir * speed * Time.deltaTime;

        if (t >= 1f) { t = 1f; dir = -1; }
        else if (t <= 0f) { t = 0f; dir = 1; }

        ApplyMarkerPosition();
    }

    private void ApplyMarkerPosition()
    {
        if (!trackArea || !marker) return;
        float y = Mathf.Lerp(GetTrackBottom(), GetTrackTop(), t);
        var pos = marker.anchoredPosition;
        pos.y = y;
        marker.anchoredPosition = pos;
    }

    private void Evaluate()
    {
        state = State.Result;

        bool success = IsMarkerInsideZone();
        if (success)
        {
            if (resultText) resultText.text = "Fish Caught!";
            onCatch?.Invoke();
        }
        else
        {
            if (resultText) resultText.text = "The fish got away…";
            onMiss?.Invoke();
        }
    }

    private bool IsMarkerInsideZone()
    {
        if (!marker || !successZone) return false;

        float markerY = marker.anchoredPosition.y;
        float zoneHalf = successZone.rect.height * 0.5f;
        float zoneCenter = successZone.anchoredPosition.y;
        float zoneMin = zoneCenter - zoneHalf;
        float zoneMax = zoneCenter + zoneHalf;

        return markerY >= zoneMin && markerY <= zoneMax;
    }

    private void RandomizeZone()
    {
        if (!trackArea || !successZone) return;

        float trackH = trackArea.rect.height;
        float zoneFrac = Random.Range(zoneSizeRange.x, zoneSizeRange.y);
        float zoneH = Mathf.Clamp(zoneFrac, 0.05f, 0.9f) * trackH;

        float minCenter = Mathf.Lerp(GetTrackBottom(), GetTrackTop(), zoneCenterClamp.x);
        float maxCenter = Mathf.Lerp(GetTrackBottom(), GetTrackTop(), zoneCenterClamp.y);
        float centerY = Random.Range(minCenter, maxCenter);

        var size = successZone.sizeDelta; size.y = zoneH; successZone.sizeDelta = size;

        var pos = successZone.anchoredPosition;
        pos.y = Mathf.Clamp(centerY, GetTrackBottom() + zoneH * 0.5f, GetTrackTop() - zoneH * 0.5f);
        successZone.anchoredPosition = pos;
    }

    private float GetTrackBottom() => -trackArea.rect.height * 0.5f;
    private float GetTrackTop() => trackArea.rect.height * 0.5f;

    private bool ValidateRefs()
    {
        if (!trackArea || !marker || !successZone)
        {
            Debug.LogError("[FishingMinigame_Input] Missing references. Assign Track Area, Marker, and Success Zone.");
            return false;
        }
        return true;
    }

    // Optional button hooks
    public void PressStop() => OnStopPerformed(default);
    public void Retry() => StartFishing();
}
