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

    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Fade Duration")]
    [SerializeField] private float fadeOutDuration;
    [SerializeField] private float fadeInDuration;

    [Header("Automatically fade back in")]
    [SerializeField] private bool automaticallyFadeBackIn = true;
    [SerializeField] private float automaticallyFadeBackInDelay = 6f;

    ///<summary>
    /// Start a Fade In.
    /// <param name="fadeOutCompletedCallback">Optional callback after fade out is completed.</param>
    /// <param name="fadeBackInCompletedCallback">Optional callback after fade back in is completed.</param>
    ///</summary>
    public void FadeOut(UnityAction fadeOutCompletedCallback = null, UnityAction fadeBackInCompletedCallback = null)
    {
        if (activeFade != null)
            StopCoroutine(activeFade);

        activeFade = StartCoroutine(FadeProcess(true, fadeOutCompletedCallback, fadeBackInCompletedCallback));
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

    private IEnumerator FadeProcess(bool fadeOut, UnityAction callback, UnityAction nextCallback = null)
    {
        float startTime = Time.time;
        float fadePercentage = 0;

        while (fadePercentage < 1)
        {
            if (fadeOut)
            {
                fadePercentage = (Time.time - startTime) / fadeOutDuration;
                canvasGroup.alpha = fadePercentage;
            }
            else
            {
                fadePercentage = (Time.time - startTime) / fadeInDuration;
                canvasGroup.alpha = 1 - fadePercentage;
            }

            yield return null;
        }

        if (callback != null)
            callback.Invoke();

        activeFade = null;

        if (fadeOut && automaticallyFadeBackIn)
        {
            yield return new WaitForSeconds(automaticallyFadeBackInDelay);
            FadeIn(nextCallback);
        }
    }
}
