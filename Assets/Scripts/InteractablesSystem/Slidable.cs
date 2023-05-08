using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class Slidable : Grabbable
{
    protected override string DefaultInteractionText => "Slide";

    [SerializeField] private SliderContext sliderContext;
    [SerializeField] private float maxSlideSpeed = 5f;


    private Vector3 targetPosition;
    private bool interactionActive;
    private Rigidbody rigidbody;
    [SerializeField] private float multi = 5f;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        rigidbody.useGravity = false;
        rigidbody.interpolation = RigidbodyInterpolation.Extrapolate;
        rigidbody.mass = 0;
    }

    void Update() => transform.localRotation = Quaternion.identity;

    protected override void InternalHandleInteract()
    {
        base.InternalHandleInteract();
        interactionActive = true;
        rigidbody.isKinematic = false;
        targetPosition = rigidbody.position;

        Debug.Log("enter");
    }

    public override void HandleUpdate()
    {
        Plane dragPlane = new Plane(sliderContext.transform.forward, sliderContext.transform.position);
        Camera cam = Camera.main;//Camera.main.transform.GetComponentsInChildren<Camera>()[1];
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (dragPlane.Raycast(ray, out float hitDistance))
        {
            Vector3 hitPoint = ray.GetPoint(hitDistance);
            targetPosition = sliderContext.ClampPosition(hitPoint);
        }
        else
        {
            targetPosition = rigidbody.position;
        }

        Debug.DrawLine(Camera.main.transform.position, targetPosition, Color.red);
        Debug.DrawLine(rigidbody.position, rigidbody.position + (rigidbody.velocity * 4f), Color.yellow);
    }

    protected override void HandleStopInteract()
    {
        interactionActive = false;
        rigidbody.isKinematic = true;
        rigidbody.velocity = Vector3.zero;

        rigidbody.MovePosition(sliderContext.SnapPosition(rigidbody.position));

        Debug.Log("exit");
    }

    void FixedUpdate()
    {
        if (interactionActive)
        {
            Vector3 moveDirection = targetPosition - rigidbody.position;
            Debug.DrawLine(rigidbody.position, rigidbody.position + moveDirection, Color.green);   
            Vector3 moveVelocity = moveDirection.normalized * maxSlideSpeed * Time.fixedDeltaTime;

            if((moveVelocity.magnitude * Time.fixedDeltaTime) > moveDirection.magnitude)
            {
                rigidbody.velocity = Vector3.zero;
                return;
            }                
            
            rigidbody.velocity = moveVelocity;      
        }
        else
        {
            rigidbody.MovePosition(sliderContext.SnapPosition(rigidbody.position));
        }
    }

}
