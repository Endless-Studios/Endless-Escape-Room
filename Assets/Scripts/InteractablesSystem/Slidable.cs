using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Slidable : Grabbable
{
    protected override string DefaultInteractionText => "Slide";

    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private SlidableContext slidableContext;
    [SerializeField] private float slideSpeed = 10f;
    [SerializeField, Min(.05f)] private float smoothSnapTime = .1f;

    [HideInInspector] public UnityEvent<Vector2> OnPositionMoved = new UnityEvent<Vector2>(); //Move finished event, Vector2 result in context's local space
    [HideInInspector] public UnityEvent<Vector2Int> OnPositionSnapped = new UnityEvent<Vector2Int>(); //Snap finished, Vector2Int x,y snap index

    private Vector3 targetPosition;
    private bool interactionActive;
    private Coroutine activeSnapCoroutine;

    private bool cachedRigidbodyIsKinematic;

    void Awake()
    {
        if (slidableContext == null)
        {
            Debug.LogWarning("Slidable Context missing. Removing Slidable.");
            Destroy(this);
            return;
        }

        cachedRigidbodyIsKinematic = rigidbody.isKinematic;

        if(rigidbody.interpolation == RigidbodyInterpolation.Interpolate)
            rigidbody.interpolation = RigidbodyInterpolation.Extrapolate; //RigidbodyInterpolation.Interpolate doesnt work properly on child rigidbodies
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
        rigidbody.isKinematic = false; //allow movement from the rigidbody's velocity
        targetPosition = rigidbody.position; //init target pos
    }

    public override void HandleUpdate()
    {
        //Plane based on the slidercontext's orientation
        Plane dragPlane = new Plane(slidableContext.transform.forward, slidableContext.transform.position);
        Camera cam = Camera.main;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        //Raycast against the plane to get the desired drag position
        if (dragPlane.Raycast(ray, out float hitDistance))
        {
            Vector3 hitPoint = ray.GetPoint(hitDistance);
            targetPosition = slidableContext.ClampPosition(hitPoint); //Clamp the desired drag position to the context's bounds
        }
        else
        {
            targetPosition = rigidbody.position;
        }

        Debug.DrawLine(cam.transform.position, targetPosition, Color.red);
    }

    protected override void HandleStopInteract()
    {
        interactionActive = false;
        rigidbody.isKinematic = cachedRigidbodyIsKinematic;
        rigidbody.velocity = Vector3.zero;

        activeSnapCoroutine = StartCoroutine(SmoothToPosition(slidableContext.GetEndSlideResult(transform.position))); //smooth to final position based on snap result
    }

    IEnumerator SmoothToPosition(SlidableContext.EndSlideResult snapResult)
    {
        Vector3 startPosition = transform.position;

        for(float elapsedTime = 0; elapsedTime < smoothSnapTime; elapsedTime += Time.deltaTime)
        {
            float lerpT = elapsedTime / smoothSnapTime;
            transform.position = (Vector3.Lerp(startPosition, snapResult.worldPosition, lerpT));
            yield return null;
        }

        transform.position = snapResult.worldPosition;

        if (snapResult.snap)
            OnPositionSnapped.Invoke(snapResult.snapID);

        OnPositionMoved.Invoke(snapResult.localPosition);
        // Debug.Log($"Snap: { snapResult.snapIndex } , Pos: { snapResult.localPosition }");
        activeSnapCoroutine = null;
    }

    void FixedUpdate()
    {
        if (interactionActive)
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
                rigidbody.velocity = moveVelocity; //move the slidable respecting physics
            }
        }
    }
}
