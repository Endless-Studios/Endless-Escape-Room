using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TransformAnimator : MonoBehaviour
{
    [SerializeField] float animationTime = 3;
    [SerializeField] Vector3 finalLocalPosition;
    [SerializeField] Vector3 finalLocalRotation;
    [SerializeField] Vector3 finalLocalScale = Vector3.one;
    [SerializeField] Transform targetTransform;

    public UnityEvent OnAnimationFinished = new UnityEvent();
    public UnityEvent OnReverseAnimationFinished = new UnityEvent();

    bool isAnimatedOrAnimating = false;
    bool desiresEndState = false;

    Vector3 startLocalPosition;
    Quaternion startLocalRotation;
    Vector3 startLocalScale;

    Coroutine currentAnimation = null;

    private void Awake()
    {
        startLocalPosition = targetTransform.localPosition;
        startLocalRotation = targetTransform.localRotation;
        startLocalScale = targetTransform.localScale;
    }

    private void OnValidate()
    {
        if(targetTransform == null)
            targetTransform = transform;
    }

    public void Animate()
    {
        if(isAnimatedOrAnimating == false)
        {
            desiresEndState = true;
            if(currentAnimation == null)
                currentAnimation = StartCoroutine(Animate(true));
        }
    }

    public void Reverse()
    {
        if(isAnimatedOrAnimating)
        {
            desiresEndState = false;
            if(currentAnimation == null)
                currentAnimation = StartCoroutine(Animate(false));
        }
    }

    IEnumerator Animate(bool forward)
    {
        isAnimatedOrAnimating = forward;
        Vector3 fromPosition = forward ? startLocalPosition : finalLocalPosition;
        Vector3 toPosition = forward ? finalLocalPosition : startLocalPosition;
        Quaternion fromRotation = forward ? startLocalRotation : Quaternion.Euler(finalLocalRotation);
        Quaternion toRotation = forward ? Quaternion.Euler(finalLocalRotation) : startLocalRotation;
        Vector3 fromScale = forward ? startLocalScale : finalLocalScale;
        Vector3 toScale = forward ? finalLocalScale : startLocalScale;

        for(float elapsedTime = 0; elapsedTime < animationTime; elapsedTime += Time.deltaTime)
        {
            targetTransform.localPosition = Vector3.Slerp(fromPosition, toPosition, elapsedTime / animationTime);
            targetTransform.localRotation = Quaternion.Slerp(fromRotation, toRotation, elapsedTime / animationTime);
            targetTransform.localScale = Vector3.Slerp(fromScale, toScale, elapsedTime / animationTime);
            yield return null;
        }
        targetTransform.localPosition = toPosition;
        targetTransform.localRotation = toRotation;
        targetTransform.localScale = toScale;
        yield return null;

        if(forward)
            OnAnimationFinished.Invoke();
        else
            OnReverseAnimationFinished.Invoke();

        if(forward && !desiresEndState)
        {
            //Reverse!
            currentAnimation = StartCoroutine(Animate(false));
        }
        else if (!forward && desiresEndState)
        {
            //Animate!
            currentAnimation = StartCoroutine(Animate(true));
        }
        else
            currentAnimation = null;
    }
}
