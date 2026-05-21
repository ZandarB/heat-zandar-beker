using System;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance;
    [SerializeField] private AudioSource audioSource;

    // Ambience
    [SerializeField] private AudioClip furnaceRoom;

    // Sound Effects
    [SerializeField] private AudioClip furnaceEnter;
    [SerializeField] private AudioClip playerJump;
    [SerializeField] private AudioClip oreMining;
    [SerializeField] private AudioClip picPickup;
    
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(string soundType)
    {
        switch (soundType)
        {
            // Ambience
            case "furnaceRoom":
                audioSource.PlayOneShot(furnaceRoom);
                break;

            // Effects
            case "furnaceEnter":
                audioSource.PlayOneShot(furnaceEnter);
                break;
            case "playerJump":
                audioSource.PlayOneShot(playerJump);
                break;
             case "oreMining":
                audioSource.PlayOneShot(oreMining);
                break;
            case "picPickup":
                audioSource.PlayOneShot(picPickup);
                break;

            default:
                break;
        }
    }
}
