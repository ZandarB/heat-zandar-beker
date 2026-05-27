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
    // Update is called once per frame
    void Update()
    {
        
    }
}
