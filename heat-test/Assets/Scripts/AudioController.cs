using System;
using UnityEngine;

public class AudioController : MonoBehaviour
{   
    // Controllers
    public static AudioController Instance;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource ambienceSource;

    // Ambience
    [SerializeField] private AudioClip furnaceRoom;
    [SerializeField] private AudioClip cavernsArea;
    
    // Sound Effects
    [SerializeField] private AudioClip unequippedItem;
    [SerializeField] private AudioClip furnaceEnter;
    [SerializeField] private AudioClip playerJump;
    [SerializeField] private AudioClip oreMining;
    [SerializeField] private AudioClip picPickup;
    [SerializeField] private AudioClip drillPickup;
    [SerializeField] private AudioClip depleteOre;
    [SerializeField] private AudioClip openToolWheel;
    [SerializeField] private AudioClip closeToolWheel;  
    [SerializeField] private AudioClip skillUpgrade;
    [SerializeField] private AudioClip skillTreeOpen;
    [SerializeField] private AudioClip skillTreeClose;
    [SerializeField] private AudioClip uiButton;
    
    void Awake()
    {
        Instance = this;
    }

    public void PlaySound(string soundType)
    {
        switch (soundType)
        {
            // Ambience
            case "furnaceRoom":
                if (!ambienceSource.isPlaying)
                {
                    ambienceSource.clip = furnaceRoom;
                    ambienceSource.loop = true;
                    ambienceSource.volume = 2f;
                    ambienceSource.Play();
                }
                break;

                 case "cavernsArea":
                if (!ambienceSource.isPlaying)
                {
                    ambienceSource.clip = cavernsArea;
                    ambienceSource.loop = true;
                    ambienceSource.volume = 0.1f;
                    ambienceSource.Play();
                }
                break;

            // Effects
            case "furnaceEnter":
                sfxSource.PlayOneShot(furnaceEnter, 3f);
                break;
            case "playerJump":
                sfxSource.PlayOneShot(playerJump, 2f);
                break;
             case "oreMining":
                sfxSource.PlayOneShot(oreMining);
                break;
             case "depleteOre":
                sfxSource.PlayOneShot(depleteOre, 2f);
                break;
            case "picPickup":
                sfxSource.PlayOneShot(picPickup, 3f);
                break;
            case "drillPickup":
                sfxSource.PlayOneShot(drillPickup);
                break;
            case "unequippedItem":
                sfxSource.PlayOneShot(unequippedItem);
                break;
            case "skillUpgrade":
                sfxSource.PlayOneShot(skillUpgrade);
                break;
            case "skillTreeOpen":
                sfxSource.PlayOneShot(skillTreeOpen);
                break;
            case "skillTreeClose":
                sfxSource.PlayOneShot(skillTreeClose);
                break;
            case "uiButton":
                sfxSource.PlayOneShot(uiButton);
                break;

            default:
                break;
        }
    }

    public void StopSound(String soundName)
    {
        if (soundName == "furnaceRoom" || soundName == "cavernArea")
        {
            ambienceSource.Stop();
        }
    }
}
