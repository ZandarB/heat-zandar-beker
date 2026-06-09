using UnityEngine;

public class UpgradeController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] public GameObject upgradeCanvas;  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void MovementUpgrade()
    {
        if(playerController.saltNum < 50f)
        {
            return;
        }
        else
        {
            AudioController.Instance.PlaySound("skillUpgrade");
            playerController.saltNum -= 50f;
            playerController.speed += 5f;
        Debug.Log("Movement Upgrade Purchased! Current Speed: " + playerController.speed);
        }
    }
    public void HeatUprgrade()
    {
        if (playerController.saltNum < 50f)
        {
            return;
        }
        else
        {
            AudioController.Instance.PlaySound("skillUpgrade");
            playerController.saltNum -= 50f;
            playerController.getProgress().maxValue += 10f;
            playerController.getProgress().currentValue = playerController.getProgress().maxValue;

            Debug.Log("Heat Upgrade Purchased! Current Max Heat: " + playerController.getProgress().maxValue);
        }
    }
    public void closeUpgrade()
    {
        AudioController.Instance.PlaySound("skillTreeClose");
        upgradeCanvas.SetActive(false);
    }
}
