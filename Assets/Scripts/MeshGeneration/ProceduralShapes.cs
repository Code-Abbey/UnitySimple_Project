using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Static utility for generating procedural meshes at runtime.
/// All methods create a fresh Mesh with vertices, triangles, UVs, and recalculated normals.
/// </summary>
public static class ProceduralShapes
{
    // ----------------------------------------------------------------------
    // CUBE - Convex
    // ----------------------------------------------------------------------
    public static Mesh CreateCube(float size = 1f)
    {
        Mesh mesh = new Mesh { name = "ProceduralCube" };
        float s = size * 0.5f;

        // 24 vertices (4 per face) so each face gets its own normals and UVs
        Vector3[] vertices = {
            // Front (+Z)
            new Vector3(-s, -s,  s), new Vector3( s, -s,  s),
            new Vector3( s,  s,  s), new Vector3(-s,  s,  s),
            // Back (-Z)
            new Vector3( s, -s, -s), new Vector3(-s, -s, -s),
            new Vector3(-s,  s, -s), new Vector3( s,  s, -s),
            // Top (+Y)
            new Vector3(-s,  s,  s), new Vector3( s,  s,  s),
            new Vector3( s,  s, -s), new Vector3(-s,  s, -s),
            // Bottom (-Y)
            new Vector3(-s, -s, -s), new Vector3( s, -s, -s),
            new Vector3( s, -s,  s), new Vector3(-s, -s,  s),
            // Right (+X)
            new Vector3( s, -s,  s), new Vector3( s, -s, -s),
            new Vector3( s,  s, -s), new Vector3( s,  s,  s),
            // Left (-X)
            new Vector3(-s, -s, -s), new Vector3(-s, -s,  s),
            new Vector3(-s,  s,  s), new Vector3(-s,  s, -s),
        };

        int[] triangles = new int[36];
        for (int i = 0; i < 6; i++)
        {
            int v = i * 4;
            int t = i * 6;
            triangles[t]     = v;
            triangles[t + 1] = v + 1;
            triangles[t + 2] = v + 2;
            triangles[t + 3] = v;
            triangles[t + 4] = v + 2;
            triangles[t + 5] = v + 3;
        }

        Vector2[] uvs = new Vector2[24];
        for (int i = 0; i < 6; i++)
        {
            int v = i * 4;
            uvs[v]     = new Vector2(0, 0);
            uvs[v + 1] = new Vector2(1, 0);
            uvs[v + 2] = new Vector2(1, 1);
            uvs[v + 3] = new Vector2(0, 1);
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    // ----------------------------------------------------------------------
    // PYRAMID - Convex (square base)
    // ----------------------------------------------------------------------
    public static Mesh CreatePyramid(float baseSize = 1f, float height = 1.5f)
    {
        Mesh mesh = new Mesh { name = "ProceduralPyramid" };
        float s = baseSize * 0.5f;

        Vector3 apex = new Vector3(0, height, 0);
        Vector3 b1 = new Vector3(-s, 0, -s);
        Vector3 b2 = new Vector3( s, 0, -s);
        Vector3 b3 = new Vector3( s, 0,  s);
        Vector3 b4 = new Vector3(-s, 0,  s);

        Vector3[] vertices = {
            // Base (separate verts so normal can point down)
            b1, b2, b3, b4,
            // Side 1 (front, +Z)
            b4, b3, apex,
            // Side 2 (right, +X)
            b3, b2, apex,
            // Side 3 (back, -Z)
            b2, b1, apex,
            // Side 4 (left, -X)
            b1, b4, apex
        };

        // Base winding 0,1,2 / 0,2,3 makes the normal point in -Y (downward)
        int[] triangles = {
            0, 1, 2,  0, 2, 3,
            4, 5, 6,
            7, 8, 9,
            10, 11, 12,
            13, 14, 15
        };

        Vector2[] uvs = {
            new Vector2(0,0), new Vector2(1,0), new Vector2(1,1), new Vector2(0,1),
            new Vector2(0,0), new Vector2(1,0), new Vector2(0.5f,1),
            new Vector2(0,0), new Vector2(1,0), new Vector2(0.5f,1),
            new Vector2(0,0), new Vector2(1,0), new Vector2(0.5f,1),
            new Vector2(0,0), new Vector2(1,0), new Vector2(0.5f,1)
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    // ----------------------------------------------------------------------
    // SPHERE - Convex (UV sphere)
    // ----------------------------------------------------------------------
    public static Mesh CreateSphere(float radius = 0.5f, int longitudeSegments = 24, int latitudeSegments = 16)
    {
        Mesh mesh = new Mesh { name = "ProceduralSphere" };
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        for (int lat = 0; lat <= latitudeSegments; lat++)
        {
            float theta = lat * Mathf.PI / latitudeSegments;
            float sinT = Mathf.Sin(theta);
            float cosT = Mathf.Cos(theta);

            for (int lon = 0; lon <= longitudeSegments; lon++)
            {
                float phi = lon * 2f * Mathf.PI / longitudeSegments;
                float sinP = Mathf.Sin(phi);
                float cosP = Mathf.Cos(phi);

                vertices.Add(new Vector3(
                    radius * cosP * sinT,
                    radius * cosT,
                    radius * sinP * sinT));
                uvs.Add(new Vector2((float)lon / longitudeSegments, 1f - (float)lat / latitudeSegments));
            }
        }

        for (int lat = 0; lat < latitudeSegments; lat++)
        {
            for (int lon = 0; lon < longitudeSegments; lon++)
            {
                int first = lat * (longitudeSegments + 1) + lon;
                int second = first + longitudeSegments + 1;

                triangles.Add(first);
                triangles.Add(second);
                triangles.Add(first + 1);

                triangles.Add(second);
                triangles.Add(second + 1);
                triangles.Add(first + 1);
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    // ----------------------------------------------------------------------
    // TORUS - Concave (has a hole through the middle)
    // ----------------------------------------------------------------------
    public static Mesh CreateTorus(float majorRadius = 1f, float minorRadius = 0.3f,
                                   int majorSegments = 32, int minorSegments = 16)
    {
        Mesh mesh = new Mesh { name = "ProceduralTorus" };
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        for (int i = 0; i <= majorSegments; i++)
        {
            float u = (float)i / majorSegments;
            float theta = u * 2f * Mathf.PI;
            float cosTheta = Mathf.Cos(theta);
            float sinTheta = Mathf.Sin(theta);

            for (int j = 0; j <= minorSegments; j++)
            {
                float v = (float)j / minorSegments;
                float phi = v * 2f * Mathf.PI;
                float cosPhi = Mathf.Cos(phi);
                float sinPhi = Mathf.Sin(phi);

                float x = (majorRadius + minorRadius * cosPhi) * cosTheta;
                float y = minorRadius * sinPhi;
                float z = (majorRadius + minorRadius * cosPhi) * sinTheta;

                vertices.Add(new Vector3(x, y, z));
                uvs.Add(new Vector2(u, v));
            }
        }

        for (int i = 0; i < majorSegments; i++)
        {
            for (int j = 0; j < minorSegments; j++)
            {
                int a = i * (minorSegments + 1) + j;
                int b = a + minorSegments + 1;

                triangles.Add(a);
                triangles.Add(b);
                triangles.Add(a + 1);

                triangles.Add(b);
                triangles.Add(b + 1);
                triangles.Add(a + 1);
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    // ----------------------------------------------------------------------
    // L-SHAPE - Concave (inward angle)
    // ----------------------------------------------------------------------
    public static Mesh CreateLShape(float size = 1f, float thickness = 0.4f, float depth = 0.5f)
    {
        Mesh mesh = new Mesh { name = "ProceduralLShape" };
        float s = size;
        float t = thickness;
        float d = depth * 0.5f;

        // 2D profile traced counter-clockwise (viewed from +Z)
        Vector2[] profile = {
            new Vector2(0, 0),
            new Vector2(s, 0),
            new Vector2(s, t),
            new Vector2(t, t),     // <-- inner concave corner
            new Vector2(t, s),
            new Vector2(0, s)
        };

        // Center the profile around origin
        Vector2 c = new Vector2(s * 0.5f, s * 0.5f);
        for (int i = 0; i < profile.Length; i++) profile[i] -= c;

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        // Front face (+Z), fan-triangulated from profile[0]
        int frontStart = vertices.Count;
        for (int i = 0; i < profile.Length; i++)
        {
            vertices.Add(new Vector3(profile[i].x, profile[i].y, d));
            uvs.Add(new Vector2(profile[i].x / s + 0.5f, profile[i].y / s + 0.5f));
        }
        for (int i = 1; i < profile.Length - 1; i++)
        {
            triangles.Add(frontStart);
            triangles.Add(frontStart + i);
            triangles.Add(frontStart + i + 1);
        }

        // Back face (-Z), reversed winding
        int backStart = vertices.Count;
        for (int i = 0; i < profile.Length; i++)
        {
            vertices.Add(new Vector3(profile[i].x, profile[i].y, -d));
            uvs.Add(new Vector2(profile[i].x / s + 0.5f, profile[i].y / s + 0.5f));
        }
        for (int i = 1; i < profile.Length - 1; i++)
        {
            triangles.Add(backStart);
            triangles.Add(backStart + i + 1);
            triangles.Add(backStart + i);
        }

        // Side faces (extrude profile edges along Z)
        for (int i = 0; i < profile.Length; i++)
        {
            int next = (i + 1) % profile.Length;
            int sideStart = vertices.Count;

            vertices.Add(new Vector3(profile[i].x,    profile[i].y,     d));
            vertices.Add(new Vector3(profile[next].x, profile[next].y,  d));
            vertices.Add(new Vector3(profile[next].x, profile[next].y, -d));
            vertices.Add(new Vector3(profile[i].x,    profile[i].y,    -d));

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(0, 1));

            triangles.Add(sideStart);
            triangles.Add(sideStart + 2);
            triangles.Add(sideStart + 1);
            triangles.Add(sideStart);
            triangles.Add(sideStart + 3);
            triangles.Add(sideStart + 2);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    // ----------------------------------------------------------------------
    // STAR - Concave (inward points between outer points)
    // ----------------------------------------------------------------------
    public static Mesh CreateStar(int points = 5, float outerRadius = 1f,
                                  float innerRadius = 0.4f, float depth = 0.3f)
    {
        Mesh mesh = new Mesh { name = "ProceduralStar" };
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        int totalPoints = points * 2;
        Vector2[] profile = new Vector2[totalPoints];

        for (int i = 0; i < totalPoints; i++)
        {
            float angle = (i * Mathf.PI * 2f / totalPoints) - Mathf.PI * 0.5f;
            float radius = (i % 2 == 0) ? outerRadius : innerRadius;
            profile[i] = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
        }

        float d = depth * 0.5f;

        // Front face fan
        int frontCenterIdx = vertices.Count;
        vertices.Add(new Vector3(0, 0, d));
        uvs.Add(new Vector2(0.5f, 0.5f));

        int frontFirstIdx = vertices.Count;
        for (int i = 0; i < totalPoints; i++)
        {
            vertices.Add(new Vector3(profile[i].x, profile[i].y, d));
            uvs.Add(new Vector2(profile[i].x / (outerRadius * 2f) + 0.5f,
                                profile[i].y / (outerRadius * 2f) + 0.5f));
        }
        for (int i = 0; i < totalPoints; i++)
        {
            triangles.Add(frontCenterIdx);
            triangles.Add(frontFirstIdx + i);
            triangles.Add(frontFirstIdx + ((i + 1) % totalPoints));
        }

        // Back face fan (reversed winding)
        int backCenterIdx = vertices.Count;
        vertices.Add(new Vector3(0, 0, -d));
        uvs.Add(new Vector2(0.5f, 0.5f));

        int backFirstIdx = vertices.Count;
        for (int i = 0; i < totalPoints; i++)
        {
            vertices.Add(new Vector3(profile[i].x, profile[i].y, -d));
            uvs.Add(new Vector2(profile[i].x / (outerRadius * 2f) + 0.5f,
                                profile[i].y / (outerRadius * 2f) + 0.5f));
        }
        for (int i = 0; i < totalPoints; i++)
        {
            triangles.Add(backCenterIdx);
            triangles.Add(backFirstIdx + ((i + 1) % totalPoints));
            triangles.Add(backFirstIdx + i);
        }

        // Side faces around the star perimeter
        for (int i = 0; i < totalPoints; i++)
        {
            int next = (i + 1) % totalPoints;
            int sideStart = vertices.Count;

            vertices.Add(new Vector3(profile[i].x,    profile[i].y,     d));
            vertices.Add(new Vector3(profile[next].x, profile[next].y,  d));
            vertices.Add(new Vector3(profile[next].x, profile[next].y, -d));
            vertices.Add(new Vector3(profile[i].x,    profile[i].y,    -d));

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(0, 1));

            triangles.Add(sideStart);
            triangles.Add(sideStart + 2);
            triangles.Add(sideStart + 1);
            triangles.Add(sideStart);
            triangles.Add(sideStart + 3);
            triangles.Add(sideStart + 2);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }
}
