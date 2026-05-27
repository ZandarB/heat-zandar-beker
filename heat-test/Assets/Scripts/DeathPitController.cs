using UnityEngine;

public class DeathPitController : MonoBehaviour
{
    private Transform spawnPoint;

    private void Start()
    {
        // Find the respawn point in the scene
        GameObject respawnObject = GameObject.FindGameObjectWithTag("Respawn");

        if (respawnObject != null)
        {
            spawnPoint = respawnObject.transform;
        }
        else
        {
            Debug.LogError("No object with tag 'Respawn' found.");
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            CharacterController controller = collision.GetComponent<CharacterController>();

            if (controller != null && spawnPoint != null)
            {
                // Disable controller before teleporting
                controller.enabled = false;

                // Move player to respawn point
                collision.transform.position = spawnPoint.position;

                // Re-enable controller
                controller.enabled = true;

                Debug.Log("Respawn");
            }
        }
    }
}