using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GifPlayer : MonoBehaviour
{
    public Image image;           
    public Sprite[] frames;        
    public float frameRate = 0.1f;  

    private int currentFrame;
    private Coroutine gifCoroutine;

    void OnEnable()
    {
        currentFrame = 0;
        if (frames != null && frames.Length > 0)
            gifCoroutine = StartCoroutine(PlayGif());
    }

    void OnDisable()
    {
        if (gifCoroutine != null)
        {
            StopCoroutine(gifCoroutine);
            gifCoroutine = null;
        }
    }

    IEnumerator PlayGif()
    {
        while (true)
        {
            image.sprite = frames[currentFrame];
            currentFrame = (currentFrame + 1) % frames.Length;
            yield return new WaitForSeconds(frameRate);
        }
    }
}
