using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DrillMinigame : MonoBehaviour
{
    public static DrillMinigame Instance;

    public enum GameState { Idle, Waiting, Running, Result }

    [SerializeField] RectTransform playingArea;
    [SerializeField] RectTransform player;
    [SerializeField] RectTransform winArea;
    [SerializeField] TextMeshProUGUI text;

    public GameObject targetWall;

    public Vector2 zoneCenterClamp = new Vector2(0.15f, 0.85f);
    public Vector2 zoneSizeRange = new Vector2(0.18f, 0.32f);

    float t;
    int dir = 1;
    float speed = 1.5f;
    float timer = 1f;

    

    private GameState state = GameState.Idle;

    int hitsRequired;
    int currentHits;


    private void OnEnable()
    {
        hitsRequired = Random.Range(1, 5);
        currentHits = 0;

        text.text = $"Hits Remaining: {hitsRequired - currentHits}";

        ResetGame();
    }

    public void AssignWall(GameObject target)
    {
        targetWall = target;
    }

    private void Update()
    {
        switch (state)
        {
            case GameState.Waiting:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    state = GameState.Running;
                }
                break;

            case GameState.Running:
                UpdateMarker();
                break;
        }
    }
    public void OnExternalInteract()
    {
        Debug.Log("Minigame input");

        if (state == GameState.Running)
            Evaluate();
    }

    private void ResetGame()
    {
        text.text = $"Hits Remaining: {hitsRequired - currentHits}";
        if (currentHits >= hitsRequired)
        {
            Destroy(targetWall);
            PlayerController.Instance.gameHasStarted = false;
            PlayerController.Instance.allowedToMove = true;
            PlayerController.Instance.gameUI.SetActive(true);

            gameObject.SetActive(false);
            return;
        }

        timer = 1f;
        state = GameState.Waiting;

        RandomizeZone();
        ResetMarker();
    }

    private void ResetMarker()
    {
        t = Random.Range(0.05f, 0.95f);
        dir = Random.value < 0.5f ? 1 : -1;

        ApplyMarkerPosition();
    }

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

        Vector2 pos = player.anchoredPosition;
        pos.y = y;
        player.anchoredPosition = pos;
    }
    private void RandomizeZone()
    {
        if (!playingArea || !winArea) return;

        float trackH = playingArea.rect.height;
        float zoneFrac = Random.Range(zoneSizeRange.x, zoneSizeRange.y);
        float zoneH = Mathf.Clamp(zoneFrac, 0.05f, 0.9f) * trackH;

        float minCenter = Mathf.Lerp(GetBottom(), GetTop(), zoneCenterClamp.x);
        float maxCenter = Mathf.Lerp(GetBottom(), GetTop(), zoneCenterClamp.y);
        float centerY = Random.Range(minCenter, maxCenter);

        var size = winArea.sizeDelta; size.y = zoneH; winArea.sizeDelta = size;

        var pos = winArea.anchoredPosition;
        pos.y = Mathf.Clamp(centerY, GetBottom() + zoneH * 0.5f, GetTop() - zoneH * 0.5f);
        winArea.anchoredPosition = pos;
    }


    private bool IsMarkerInsideZone()
    {
        float markerY = player.anchoredPosition.y;

        float half = winArea.rect.height * 0.5f;
        float center = winArea.anchoredPosition.y;

        return markerY >= center - half && markerY <= center + half;
    }

    private void Evaluate()
    {
        state = GameState.Result;

        bool success = IsMarkerInsideZone();
        if (success)
        {
            currentHits++;
            ResetGame();
        }
        else
        {
            currentHits = 0;
            ResetGame();
        }
    }

    private float GetBottom() => -playingArea.rect.height * 0.5f;
    private float GetTop() => playingArea.rect.height * 0.5f;
}