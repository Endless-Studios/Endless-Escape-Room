using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Slidable : Grabbable
{
    protected override string DefaultInteractionText => "Slide";
    public SlidableContext SlidableContext => slidableContext;

    [HideInInspector] public UnityEvent<Vector2> OnPositionMoved = new UnityEvent<Vector2>(); //Move finished event, Vector2 result in context's local space (or [x,z] world space for slidables w/o a context)
    [HideInInspector] public UnityEvent<Vector2Int> OnPositionSnapped = new UnityEvent<Vector2Int>(); //Snap finished, Vector2Int x,y snap index

    [SerializeField] private SlidableContext slidableContext;
    [SerializeField] private float slideSpeed = 10f;
    [SerializeField, Min(.05f)] private float smoothSnapTime = .1f;

    [Header("Rigidbody Options")]
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private bool freezeRotationDuringSlide = false;
    [SerializeField] private bool slideKinematically = false;

    private Vector3 targetPosition;
    private bool interactionActive;
    private Coroutine activeSnapCoroutine;

    private bool cachedRigidbodyIsKinematic;
    private RigidbodyConstraints cachedRigidbodyConstraints;

    private void Awake()
    {
        if (rigidbody)
        {
            cachedRigidbodyIsKinematic = rigidbody.isKinematic;
            cachedRigidbodyConstraints = rigidbody.constraints;

            if (rigidbody.interpolation == RigidbodyInterpolation.Interpolate)
                rigidbody.interpolation = RigidbodyInterpolation.Extrapolate; //RigidbodyInterpolation.Interpolate doesnt work properly on child rigidbodies
        }

        HandleStopInteract(); //Setup initial state
    }

    protected virtual void OnValidate()
    {
        if (rigidbody == null)
            rigidbody = GetComponent<Rigidbody>();
    }

    protected override void InternalHandleInteract()
    {
        base.InternalHandleInteract();

        if (activeSnapCoroutine != null) //cancel snapping
        {
            StopCoroutine(activeSnapCoroutine);
            activeSnapCoroutine = null;
        }

        interactionActive = true;

        if (rigidbody != null)
        {
            rigidbody.isKinematic = slideKinematically; //allow movement from the rigidbody's velocity
            targetPosition = rigidbody.position; //init target pos
        }
        else
        {
            targetPosition = transform.position;
        }

        if (freezeRotationDuringSlide)
            rigidbody.freezeRotation = true;
    }

    public override void HandleUpdate()
    {
        //Plane based on the slidercontext's orientation
        Plane dragPlane;

        if (slidableContext != null)
            dragPlane = slidableContext.GetContextPlane(this);
        else
            dragPlane = SlidableContext.GetDefaultPlaneAtPosition(transform.position);

        Camera cam = Camera.main;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        //Raycast against the plane to get the desired drag position
        if (dragPlane.Raycast(ray, out float hitDistance))
        {
            Vector3 hitPoint = ray.GetPoint(hitDistance);

            if (slidableContext != null)
                targetPosition = slidableContext.ClampPosition(hitPoint); //Clamp the desired drag position to the context's bounds
            else
                targetPosition = hitPoint;
        }
        else
        {
            if (rigidbody != null)
                targetPosition = rigidbody.position;
            else
                targetPosition = transform.position;
        }

        Debug.DrawLine(cam.transform.position, targetPosition, Color.red);
    }

    protected override void HandleStopInteract()
    {
        interactionActive = false;

        if (rigidbody != null)
        {
            rigidbody.isKinematic = cachedRigidbodyIsKinematic;
            rigidbody.velocity = Vector3.zero;
            rigidbody.constraints = cachedRigidbodyConstraints;
        }

        if (slidableContext != null)
            activeSnapCoroutine = StartCoroutine(SmoothToPosition(slidableContext.GetEndSlideResult(transform.position))); //smooth to final position based on snap result
        else
            OnPositionMoved.Invoke(new Vector2(transform.position.x, transform.position.z));
    }

    private IEnumerator SmoothToPosition(SlidableContext.EndSlideResult snapResult)
    {
        Vector3 startPosition = transform.position;

        for (float elapsedTime = 0; elapsedTime < smoothSnapTime; elapsedTime += Time.deltaTime)
        {
            float interpolationPoint = elapsedTime / smoothSnapTime;
            transform.position = (Vector3.Lerp(startPosition, snapResult.worldPosition, interpolationPoint));
            yield return null;
        }

        transform.position = snapResult.worldPosition;

        if (snapResult.snap)
            OnPositionSnapped.Invoke(snapResult.snapID);

        OnPositionMoved.Invoke(snapResult.localPosition);
        activeSnapCoroutine = null;
    }

    private void Update()
    {
        if (interactionActive && rigidbody == null)
        {
            Vector3 moveDirection = targetPosition - transform.position;
            Vector3 moveVelocity = moveDirection.normalized * slideSpeed * Time.fixedDeltaTime; //Time.fixedDelta to move same speed as rigidbody version. change speed calc??

            if ((moveVelocity * Time.fixedDeltaTime).sqrMagnitude > moveDirection.sqrMagnitude) //check to see if the drag will overshoot the target position
                transform.position = targetPosition;
            else
                transform.position += moveVelocity * Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (interactionActive && rigidbody != null)
        {
            Vector3 moveDirection = targetPosition - rigidbody.position;
            Vector3 moveVelocity = moveDirection.normalized * slideSpeed * Time.fixedDeltaTime;
            Debug.DrawLine(rigidbody.position, rigidbody.position + moveDirection, Color.green);

            if ((moveVelocity * Time.fixedDeltaTime).sqrMagnitude > moveDirection.sqrMagnitude) //check to see if the drag will overshoot the target position
            {
                rigidbody.velocity = Vector3.zero;
                rigidbody.MovePosition(targetPosition);
            }
            else
            {
                if (slideKinematically == false)
                    rigidbody.velocity = moveVelocity; //move the slidable respecting physics
                else
                    rigidbody.MovePosition(rigidbody.position + (moveVelocity * Time.fixedDeltaTime));
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (slidableContext != null)
            slidableContext.DrawGizmosForSelectedSlidable(this);
    }
}
