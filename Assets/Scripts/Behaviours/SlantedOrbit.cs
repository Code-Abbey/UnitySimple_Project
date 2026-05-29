using UnityEngine;

/// <summary>
/// Rotates this GameObject around a target transform along a slanted (non-axis-aligned) axis.
/// If no target is set, defaults to the parent transform.
/// </summary>
public class SlantedOrbit : MonoBehaviour
{
    [Tooltip("Transform to orbit around. If null, uses parent.")]
    public Transform target;

    [Tooltip("Orbit speed in degrees per second.")]
    public float orbitSpeed = 45f;

    [Tooltip("Axis of orbit (in world space). Will be normalized. A non-axis-aligned vector creates the 'slanted' orbit.")]
    public Vector3 orbitAxis = new Vector3(1f, 1f, 0.3f);

    [Tooltip("Also spin the object on its own axis while orbiting.")]
    public bool selfRotate = true;
    public float selfRotateSpeed = 60f;

    void Start()
    {
        if (target == null && transform.parent != null)
            target = transform.parent;
    }

    void Update()
    {
        if (target == null) return;

        Vector3 axis = orbitAxis.sqrMagnitude > 0.0001f ? orbitAxis.normalized : Vector3.up;

        // Orbit around the parent on the slanted axis
        transform.RotateAround(target.position, axis, orbitSpeed * Time.deltaTime);

        // Optional: spin in place too
        if (selfRotate)
            transform.Rotate(axis, selfRotateSpeed * Time.deltaTime, Space.Self);
    }
}
