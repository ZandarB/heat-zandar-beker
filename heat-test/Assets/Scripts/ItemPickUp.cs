using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    [SerializeField] public bool isDrill;
    [SerializeField] public bool isPick;

    // Use OnTriggerEnter � works with CharacterController
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isDrill)
            {
                AudioController.Instance.PlaySound("drillPickup");
                PlayerController.Instance.PickUpDrill();
                Debug.Log("Drill destroy");
            }
            else if (isPick)
            {   AudioController.Instance.PlaySound("picPickup");
                PlayerController.Instance.PickUpPick();
            }
            Destroy(gameObject);
        }
    }
}