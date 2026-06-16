using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DrillMinigame : MonoBehaviour
{
    public static DrillMinigame Instance;

    public enum GameState { Idle, Waiting, Running, Result }

    private GameState state = GameState.Idle;



    private int remainingHits;

    private float timer;
    private float t;
    private int dir = 1;

    private void OnEnable()
    {
        Reset();
    }

    private void Update()
    {
        if (state != GameState.Running) return;

        UpdateMarker();
    }

    // ================= START =================
    public void StartDrilling(WallBreakController wall)
    {
        state = GameState.Running;

        currentWall = wall;

        remainingHits = wall.GetHitsRequired();

        canvas.SetActive(true);

        resultText.text = $"Break the wall! ({remainingHits})";

        ResetRun();

        state = GameState.Waiting;
    }

    // ================= INPUT =================
    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (state == GameState.Running)
            Evaluate();
    }

    // ================= CORE LOOP =================
    private void Evaluate()
    {
        state = GameState.Result;

        bool success = IsMarkerInsideZone();

        if (success)
        {
            remainingHits--;

            if (remainingHits <= 0)
            {
                resultText.text = "Wall Broken!";

                currentWall.HitWall();

                Invoke(nameof(CloseMinigame), 1f);
                return;
            }

            resultText.text = $"Hit! ({remainingHits})";

            Invoke(nameof(NextHit), 0.5f);
        }
        else
        {
            resultText.text = "Miss!";

            Invoke(nameof(NextHit), 0.5f);
        }
    }

    private void NextHit()
    {
        ResetRun();
        state = GameState.Waiting;
    }

    private void CloseMinigame()
    {
        currentWall = null;
        state = GameState.Idle;
        canvas.SetActive(false);
    }

    // ================= RUN SETUP =================
    private void ResetRun()
    {
        timer = Random.Range(hitRange.x, hitRange.y);

        RandomizeZone();
        ResetMarker();
    }

    private void ResetMarker()
    {
        t = Random.Range(0.05f, 0.95f);
        dir = Random.value < 0.5f ? 1 : -1;

        ApplyMarkerPosition();
    }

    // ================= MOVEMENT =================
    private void UpdateMarker()
    {
        t += dir * speed * Time.deltaTime;

        if (t >= 1f) { t = 1f; dir = -1; }
        if (t <= 0f) { t = 0f; dir = 1; }

        ApplyMarkerPosition();
    }

    private void ApplyMarkerPosition()
    {
        float y = Mathf.Lerp(GetBottom(), GetTop(), t);

        Vector2 pos = marker.anchoredPosition;
        pos.y = y;
        marker.anchoredPosition = pos;
    }

    // ================= ZONE =================
    private void RandomizeZone()
    {
        float height = gameArea.rect.height;

        float zoneHeight =
            Random.Range(zoneSizeRange.x, zoneSizeRange.y) * height;

        Vector2 size = successArea.sizeDelta;
        size.y = zoneHeight;
        successArea.sizeDelta = size;

        float min = Mathf.Lerp(GetBottom(), GetTop(), zoneCenterClamp.x);
        float max = Mathf.Lerp(GetBottom(), GetTop(), zoneCenterClamp.y);

        float center = Random.Range(min, max);

        Vector2 pos = successArea.anchoredPosition;
        pos.y = Mathf.Clamp(center,
            GetBottom() + zoneHeight * 0.5f,
            GetTop() - zoneHeight * 0.5f);

        successArea.anchoredPosition = pos;
    }

    private bool IsMarkerInsideZone()
    {
        float markerY = marker.anchoredPosition.y;

        float half = successArea.rect.height * 0.5f;
        float center = successArea.anchoredPosition.y;

        return markerY >= center - half && markerY <= center + half;
    }

    private float GetBottom() => -gameArea.rect.height * 0.5f;
    private float GetTop() => gameArea.rect.height * 0.5f;
}