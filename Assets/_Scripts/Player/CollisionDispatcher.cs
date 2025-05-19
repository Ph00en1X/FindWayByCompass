using System;
using UnityEngine;

public sealed class CollisionDispatcher : MonoBehaviour
{
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private LayerMask _portalLayer;

    public event Action ObstacleHit;
    public event Action PortalHit;

    private void OnCollisionEnter2D(Collision2D c) => Evaluate(c.collider);
    private void OnTriggerEnter2D(Collider2D c) => Evaluate(c);

    private void Evaluate(Component other)
    {
        int layer = other.gameObject.layer;
        if ((_obstacleLayer.value & (1 << layer)) != 0) ObstacleHit?.Invoke();
        else if ((_portalLayer.value & (1 << layer)) != 0) PortalHit?.Invoke();
    }
}
