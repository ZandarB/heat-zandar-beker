using UnityEngine;

public class WallBreakController : MonoBehaviour
{
    [SerializeField] private int hitsRequired = 1;
    [SerializeField] PlayerController playerController;

    private int currentHits = 0;

    private void BreakWall()
    {
        playerController.gameHasStarted = false;
        Destroy(gameObject);
    }
}