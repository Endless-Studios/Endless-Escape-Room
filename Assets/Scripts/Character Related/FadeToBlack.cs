using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

///<summary>
/// Full Screen fade to black effect on the PlayerUI Canvas.
///</summary>
public class FadeToBlack : MonoBehaviour
{
    private Coroutine activeFade;

    [SerializeField] private Image fadeToBlackImage;

    [Header("Fade Duration")]
    [SerializeField] private float fadeOutDuration;
    [SerializeField] private float fadeInDuration;

    [Header("Automatically fade back in")]
    [SerializeField] private bool automaticallyFadeBackIn = true;
    [SerializeField] private float automaticallyFadeBackInDelay = 6f;

    ///<summary>
    /// Start a Fade In.
    /// <param name="callback">Optional callback after fade out is completed.</param>
    ///</summary>
    public void FadeOut(UnityAction callback = null)
    {
        if (activeFade != null)
            StopCoroutine(activeFade);

        activeFade = StartCoroutine(FadeProcess(true, callback));
    }

    ///<summary>
    /// Start a Fade In.
    /// <param name="callback">Callback after fade in is completed.</param>
    ///</summary>
    public void FadeIn(UnityAction callback)
    {
        if (activeFade != null)
            StopCoroutine(activeFade);

        activeFade = StartCoroutine(FadeProcess(false, callback));
    }

    ///<summary>
    /// Start a Fade In.
    ///</summary>
    public void FadeIn()
    {
        FadeIn(null);
    }

    private IEnumerator FadeProcess(bool fadeOut, UnityAction callback)
    {
        float startTime = Time.realtimeSinceStartup;
        float fadePercentage = 0;

        while (fadePercentage < 1)
        {
            if (fadeOut)
            {
                fadePercentage = (Time.realtimeSinceStartup - startTime) / fadeOutDuration;
                fadeToBlackImage.color = new Color(0, 0, 0, fadePercentage);
            }
            else
            {
                fadePercentage = (Time.realtimeSinceStartup - startTime) / fadeInDuration;
                fadeToBlackImage.color = new Color(0, 0, 0, 1 - fadePercentage);
            }

            yield return null;
        }

        if (callback != null)
            callback.Invoke();

        activeFade = null;

        if (fadeOut && automaticallyFadeBackIn)
            Invoke("FadeIn", automaticallyFadeBackInDelay);
    }
}