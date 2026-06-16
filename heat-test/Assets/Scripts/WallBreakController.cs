using UnityEngine;

public class WallBreakController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;

    [SerializeField] private int hitsRequired = 3;

    private int currentHits;

    public int GetHitsRequired() => hitsRequired;

    public bool HitWall()
    {
        currentHits++;

        if (currentHits >= hitsRequired)
        {
            Destroy(gameObject);
            return true;
        }

        return false;
    }
    private void BreakWall()
    {
        playerController.gameHasStarted = false;
        Destroy(gameObject);
    }
}

