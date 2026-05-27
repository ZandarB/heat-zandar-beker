using UnityEngine;

public class WaterController : MonoBehaviour
{
    GameObject waterWarning;
    private bool inWater = false;
    void Awake()
    {
        waterWarning = GameObject.FindWithTag("DamageWarning");
    }
    void Update()
    {
        if (inWater)
        {
            ProgressBarController.Instance.Decrease(Time.deltaTime * 2f);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision with water");
        if(other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player hit water");
            inWater = true;
            waterWarning.SetActive(true);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inWater = false;
            waterWarning.SetActive(false);
        }
    }
    
}
