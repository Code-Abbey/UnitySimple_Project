using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

/// <summary>
/// Casts a ray forward from this transform. When it hits a Collider, the ray reflects
/// off the surface normal and continues, up to a max number of bounces.
/// The path is drawn using a LineRenderer.
///
/// Controls:
///   - Hold left mouse / W-A-S-D arrow keys: optional rotation of the shooter
///   - Move mouse: aim (if useMouseLook is true)
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class RaycastShooter : MonoBehaviour
{
    [Header("Ray Settings")]
    [Tooltip("Maximum total length the ray can travel (sum of all bounce segments).")]
    public float maxDistance = 60f;

    [Tooltip("Maximum number of reflections before the ray stops.")]
    public int maxReflections = 5;

    [Header("Behaviour")]
    [Tooltip("If true, the ray is updated and drawn every frame automatically.")]
    public bool continuous = true;

    [Tooltip("Key to fire the ray when 'continuous' is false.")]
    public KeyCode fireKey = KeyCode.Space;

    [Header("Visual")]
    public Color rayColor = new Color(0.2f, 0.9f, 1f);
    public float lineWidth = 0.05f;
    [Tooltip("Optional small marker placed at each reflection point. Leave null to skip.")]
    public GameObject hitMarkerPrefab;

    [Header("Optional Auto-Rotation")]
    public bool autoRotate = false;
    public Vector3 autoRotateSpeed = new Vector3(0f, 25f, 0f);

    [Header("Optional Mouse Look")]
    public bool useMouseLook = false;
    public float mouseSensitivity = 200f;

    private LineRenderer lr;
    private float yaw, pitch;
    private readonly List<GameObject> activeMarkers = new List<GameObject>();

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        // Use an unlit material so the line is always visible regardless of pipeline
        Shader sh = Shader.Find("Universal Render Pipeline/Unlit");
        if (sh == null) sh = Shader.Find("Unlit/Color");
        if (sh == null) sh = Shader.Find("Sprites/Default");
        lr.material = new Material(sh);
        lr.material.color = rayColor;
        lr.startColor = rayColor;
        lr.endColor = rayColor;
        lr.useWorldSpace = true;

        Vector3 e = transform.eulerAngles;
        yaw = e.y;
        pitch = e.x;
    }

    void Update()
    {
        if (autoRotate)
            transform.Rotate(autoRotateSpeed * Time.deltaTime, Space.World);

        if (useMouseLook && Mouse.current != null)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            yaw   += delta.x * mouseSensitivity * Time.deltaTime;
            pitch -= delta.y * mouseSensitivity * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, -89f, 89f);
            transform.eulerAngles = new Vector3(pitch, yaw, 0f);
        }

        bool fireHeld    = Keyboard.current != null && Keyboard.current.spaceKey.isPressed;
        bool fireReleased = Keyboard.current != null && Keyboard.current.spaceKey.wasReleasedThisFrame;

        if (continuous || fireHeld)
            CastReflectingRay();
        else if (fireReleased)
            ClearLine();
    }

    void CastReflectingRay()
    {
        ClearMarkers();

        List<Vector3> points = new List<Vector3>();
        Vector3 origin = transform.position;
        Vector3 dir = transform.forward;
        float remaining = maxDistance;

        points.Add(origin);

        for (int i = 0; i <= maxReflections; i++)
        {
            if (Physics.Raycast(origin, dir, out RaycastHit hit, remaining))
            {
                points.Add(hit.point);
                remaining -= hit.distance;

                // Spawn a glowing spark at the bounce point (procedural — no prefab needed)
                GameObject spark = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                spark.name = "HitSpark";
                spark.transform.position = hit.point;
                spark.transform.localScale = Vector3.one * 0.18f;
                Destroy(spark.GetComponent<Collider>()); // sparks shouldn't block rays

                Shader sh = Shader.Find("Universal Render Pipeline/Lit")
                         ?? Shader.Find("Standard");
                Material sparkMat = new Material(sh);
                // Bright emissive colour so it glows
                Color sparkCol = new Color(0.3f, 1f, 0.9f);
                if (sparkMat.HasProperty("_BaseColor"))   sparkMat.SetColor("_BaseColor",   sparkCol);
                if (sparkMat.HasProperty("_Color"))       sparkMat.SetColor("_Color",       sparkCol);
                if (sparkMat.HasProperty("_EmissionColor"))
                {
                    sparkMat.EnableKeyword("_EMISSION");
                    sparkMat.SetColor("_EmissionColor", sparkCol * 2.5f);
                }
                spark.GetComponent<Renderer>().material = sparkMat;
                activeMarkers.Add(spark);

                // Also use prefab marker if one was assigned
                if (hitMarkerPrefab != null)
                {
                    GameObject m = Instantiate(hitMarkerPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                    activeMarkers.Add(m);
                }

                if (remaining <= 0f || i == maxReflections) break;

                // Reflect direction off surface normal
                dir = Vector3.Reflect(dir, hit.normal).normalized;
                // Nudge origin off the surface to avoid re-hitting it
                origin = hit.point + dir * 0.001f;
            }
            else
            {
                points.Add(origin + dir * remaining);
                break;
            }
        }

        lr.positionCount = points.Count;
        lr.SetPositions(points.ToArray());
    }

    void ClearLine()
    {
        lr.positionCount = 0;
        ClearMarkers();
    }

    void ClearMarkers()
    {
        for (int i = 0; i < activeMarkers.Count; i++)
            if (activeMarkers[i] != null) Destroy(activeMarkers[i]);
        activeMarkers.Clear();
    }
}
