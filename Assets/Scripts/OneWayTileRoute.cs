using UnityEngine;

public sealed class OneWayTileRoute : MonoBehaviour
{
    [SerializeField] private Transform sourceSurface;
    [SerializeField] private Transform restrictedSurface;
    [SerializeField] private Transform exitSurface;
    [SerializeField] private Transform finalSurface;
    [SerializeField] private Transform finalEntrySurface;
    [SerializeField] private Transform[] finalExitSurfaces;
    [SerializeField] private Vector2 surfaceHalfExtents = new Vector2(0.5f, 0.5f);

    private Rigidbody playerRigidbody;
    private Vector3 lastAllowedPosition;

    private void Awake()
    {
        if (sourceSurface == null ||
            restrictedSurface == null ||
            exitSurface == null ||
            finalSurface == null ||
            finalEntrySurface == null ||
            finalExitSurfaces == null ||
            finalExitSurfaces.Length == 0)
        {
            Debug.LogError("One-way tile route needs all route surfaces assigned.", this);
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
        bool wasOnFinalSurface = IsOnSurface(lastAllowedPosition, finalSurface);
        bool isOnFinalSurface = IsOnSurface(currentPosition, finalSurface);

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

        if (!wasOnFinalSurface &&
            isOnFinalSurface &&
            !IsOnSurface(lastAllowedPosition, finalEntrySurface))
        {
            RestoreLastAllowedPosition();
            return;
        }

        if (wasOnFinalSurface &&
            !isOnFinalSurface &&
            !IsOnAnySurface(currentPosition, finalExitSurfaces))
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

    private bool IsOnAnySurface(Vector3 position, Transform[] surfaces)
    {
        foreach (Transform surface in surfaces)
        {
            if (surface != null && IsOnSurface(position, surface))
            {
                return true;
            }
        }

        return false;
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
