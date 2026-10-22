using UnityEngine;

public sealed class OneWayTileRoute : MonoBehaviour
{
    [SerializeField] private Transform sourceSurface;
    [SerializeField] private Transform restrictedSurface;
    [SerializeField] private Transform exitSurface;
    [SerializeField] private Vector2 surfaceHalfExtents = new Vector2(0.5f, 0.5f);

    private Rigidbody playerRigidbody;
    private Vector3 lastAllowedPosition;

    private void Awake()
    {
        if (sourceSurface == null || restrictedSurface == null || exitSurface == null)
        {
            Debug.LogError("One-way tile route needs source, restricted, and exit surfaces.", this);
            enabled = false;
            return;
        }

        playerRigidbody = GetComponent<Rigidbody>();
        lastAllowedPosition = transform.position;
    }

    private void LateUpdate()
    {
        Vector3 currentPosition = transform.position;
        bool wasOnRestrictedSurface = IsOnSurface(lastAllowedPosition, restrictedSurface);
        bool isOnRestrictedSurface = IsOnSurface(currentPosition, restrictedSurface);

        if (!wasOnRestrictedSurface &&
            isOnRestrictedSurface &&
            !IsOnSurface(lastAllowedPosition, sourceSurface))
        {
            RestoreLastAllowedPosition();
            return;
        }

        if (wasOnRestrictedSurface &&
            !isOnRestrictedSurface &&
            !IsOnSurface(currentPosition, exitSurface))
        {
            RestoreLastAllowedPosition();
            return;
        }

        lastAllowedPosition = currentPosition;
    }

    private bool IsOnSurface(Vector3 position, Transform surface)
    {
        return Mathf.Abs(position.x - surface.position.x) <= surfaceHalfExtents.x &&
               Mathf.Abs(position.z - surface.position.z) <= surfaceHalfExtents.y;
    }

    private void RestoreLastAllowedPosition()
    {
        transform.position = lastAllowedPosition;

        if (playerRigidbody != null)
        {
            playerRigidbody.position = lastAllowedPosition;
        }
    }
}
