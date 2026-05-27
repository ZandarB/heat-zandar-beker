using UnityEngine;

public class FuranceController : MonoBehaviour
{
    public GameObject canvas;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canvas.SetActive(true);
        }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //AudioController.Instance.PlaySound("furnaceRoom");
    }
    void Start()
    {
        
    }

    // Update is called once per frame
        void Update()
        {
            
        }
    }
}
