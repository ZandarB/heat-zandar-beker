using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimation001 : MonoBehaviour
{
    public Image targetImage;
    public Sprite[] frames;
    public float frameRate = 12f;

    private int currentFrame = 0;
    private float timer = 0f;

    [SerializeField] private bool playOnce = false;

    void Update()
    {
        if (playOnce == false) {
            if (frames.Length == 0) return;

            timer += Time.deltaTime;

            if (timer >= 1f / frameRate)
            {
                timer = 0f;
                currentFrame = (currentFrame + 1) % frames.Length;
                targetImage.sprite = frames[currentFrame];
            }
        }
        
    }
    private Coroutine animationCoroutine;
    public void PlayAnimationOnce()
    {
        if (frames.Length == 0) return;
        currentFrame = 0;
        targetImage.sprite = frames[currentFrame];
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        animationCoroutine =
            StartCoroutine(AnimationCoroutine());
    }
    private IEnumerator AnimationCoroutine()
    {
        if (frames.Length == 0)
        {
            yield break;
        }

        float delay = 1f / frameRate;

        // Loop through all frames ONCE
        for (int i = 0; i < frames.Length; i++)
        {
            targetImage.sprite = frames[i];

            yield return new WaitForSeconds(delay);
        }

        animationCoroutine = null;
        gameObject.SetActive(false);
    }
}
