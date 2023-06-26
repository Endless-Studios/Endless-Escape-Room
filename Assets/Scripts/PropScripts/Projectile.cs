using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] GameObject hitEffect = null;
    [SerializeField] LayerMask hitLayerMask;
    [SerializeField] float damage = 10;
    [SerializeField] float speed = 10;
    [SerializeField] float maxLifeSpan = 4;

    float destroyTime;

    private void Start()
    {
        destroyTime = Time.time + maxLifeSpan;
    }

    private void Update()
    {
        RaycastHit hitInfo;
        float distanceThisFrame = Time.deltaTime * speed;
        if(Physics.Raycast(transform.position, transform.forward, out hitInfo, distanceThisFrame, hitLayerMask, QueryTriggerInteraction.Ignore))
        {
            //We hit something! See if we can do damage. Either way destroy.
            HealthComponent healthComponent = hitInfo.collider.GetComponentInParent<HealthComponent>();
            if(healthComponent != null)
                healthComponent.TakeDamage(damage);
            if(hitEffect != null)
                Instantiate(hitEffect, hitInfo.point, Quaternion.FromToRotation(transform.up, hitInfo.normal));
            Destroy(gameObject);
        }
        else if (Time.time >= destroyTime)
        {//We've been flying too long, lets clean up
            Destroy(gameObject);
        }
        else
        {//Keep moving this this frame
            transform.position += transform.forward * distanceThisFrame;
        }
    }
}
