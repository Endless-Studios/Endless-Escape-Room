using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Slidable : Grabbable
{
    protected override string DefaultInteractionText => "Slide";

    [SerializeField] private SliderContext sliderContext;
    [SerializeField] private float maxSlideSpeed = 5f;
    [SerializeField, Min(.05f)] private float smoothSnapTime = .1f;

    [SerializeField] private UnityEvent<Vector2> OnPositionMoved = new UnityEvent<Vector2>(); //Move finished event, Vector2 result in context's local space
    [SerializeField] private UnityEvent<Vector2Int> OnPositionSnapped = new UnityEvent<Vector2Int>(); //Snap finished, Vector2Int x,y snap index

    private Vector3 targetPosition;
    private bool interactionActive;
    private Rigidbody rigidbody;
    private Coroutine activeSnapCoroutine;

    void Awake()
    {
        if (sliderContext == null)
        {
            Debug.LogWarning("Slider Context missing. Removing Slidable.");
            GameObject.Destroy((Slidable)this);
            return;
        }

        //setup the rigidbody to slide properly
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        rigidbody.useGravity = false;
        rigidbody.interpolation = RigidbodyInterpolation.Extrapolate;
        rigidbody.mass = 0;
        rigidbody.drag = 0;
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
        Plane dragPlane = new Plane(sliderContext.transform.forward, sliderContext.transform.position);
        Camera cam = Camera.main;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        //Raycast against the plane to get the desired drag position
        if (dragPlane.Raycast(ray, out float hitDistance))
        {
            Vector3 hitPoint = ray.GetPoint(hitDistance);
            targetPosition = sliderContext.ClampPosition(hitPoint); //Clamp the desired drag position to the context's bounds
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
        rigidbody.isKinematic = true;
        rigidbody.velocity = Vector3.zero;

        activeSnapCoroutine = StartCoroutine(SmoothToPosition(sliderContext.EndSlide(transform.position))); //smooth to final position based on snap result
    }

    IEnumerator SmoothToPosition(SliderContext.EndSlideResult snapResult)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0;

        while (elapsedTime < smoothSnapTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpT = elapsedTime / smoothSnapTime;
            transform.position = (Vector3.Lerp(startPosition, snapResult.worldPosition, lerpT));
            yield return null;
        }

        transform.position = snapResult.worldPosition;

        if (snapResult.snap)
            OnPositionSnapped.Invoke(snapResult.snapIndex);

        OnPositionMoved.Invoke(snapResult.localPosition);
        // Debug.Log($"Snap: { snapResult.snapIndex } , Pos: { snapResult.localPosition }");
        activeSnapCoroutine = null;
    }

    void FixedUpdate()
    {
        if (interactionActive)
        {
            Vector3 moveDirection = targetPosition - rigidbody.position;
            Vector3 moveVelocity = moveDirection.normalized * maxSlideSpeed * Time.fixedDeltaTime;

            if ((moveVelocity.magnitude * Time.fixedDeltaTime) > moveDirection.magnitude) //check to see if the drag will overshoot the target position
            {
                rigidbody.velocity = Vector3.zero;
                return;
            }

            rigidbody.velocity = moveVelocity; //move the slidable respecting physics

            Debug.DrawLine(rigidbody.position, rigidbody.position + moveDirection, Color.green);
        }
    }
}
