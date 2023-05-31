using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileShooter : MonoBehaviour
{
    public UnityEvent OnFired = new UnityEvent();

    [SerializeField] Transform spawnTransform;
    [SerializeField] Projectile projectilePrefab = null;
    [SerializeField] ParticleSystem muzzleFlash;

    public void FireProjectile()
    {
        if(muzzleFlash != null)
            muzzleFlash.Play();
        Instantiate(projectilePrefab, spawnTransform.position, spawnTransform.rotation);
        OnFired.Invoke();
    }
}
