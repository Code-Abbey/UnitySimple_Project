using UnityEngine;

/// <summary>
/// Static utility for generating procedural Texture2D objects at runtime.
/// Useful so the project has no external texture asset dependencies.
/// </summary>
public static class ProceduralTextures
{
    public static Texture2D CreateCheckerboard(int size = 256, int checks = 8,
                                               Color a = default, Color b = default)
    {
        if (a == default) a = Color.white;
        if (b == default) b = Color.black;

        Texture2D tex = new Texture2D(size, size) { filterMode = FilterMode.Point };
        int checkSize = Mathf.Max(1, size / checks);
        Color[] pixels = new Color[size * size];

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                pixels[y * size + x] = ((x / checkSize) + (y / checkSize)) % 2 == 0 ? a : b;

        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    public static Texture2D CreateGradient(int size = 256, Color a = default, Color b = default)
    {
        if (a == default) a = Color.red;
        if (b == default) b = Color.blue;

        Texture2D tex = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];

        for (int y = 0; y < size; y++)
        {
            float t = (float)y / size;
            Color c = Color.Lerp(a, b, t);
            for (int x = 0; x < size; x++)
                pixels[y * size + x] = c;
        }

        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    public static Texture2D CreateNoise(int size = 256, float scale = 8f,
                                        Color a = default, Color b = default)
    {
        if (a == default) a = new Color(0.2f, 0.2f, 0.2f);
        if (b == default) b = new Color(0.85f, 0.85f, 0.85f);

        Texture2D tex = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float n = Mathf.PerlinNoise(x / (float)size * scale, y / (float)size * scale);
                pixels[y * size + x] = Color.Lerp(a, b, n);
            }

        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    public static Texture2D CreateStripes(int size = 256, int stripeCount = 8,
                                          Color a = default, Color b = default)
    {
        if (a == default) a = Color.yellow;
        if (b == default) b = Color.black;

        Texture2D tex = new Texture2D(size, size);
        int stripeWidth = Mathf.Max(1, size / stripeCount);
        Color[] pixels = new Color[size * size];

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                pixels[y * size + x] = (x / stripeWidth) % 2 == 0 ? a : b;

        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    public static Texture2D CreateRadial(int size = 256, Color inner = default, Color outer = default)
    {
        if (inner == default) inner = Color.yellow;
        if (outer == default) outer = Color.red;

        Texture2D tex = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
        float maxDist = size * 0.5f;

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float dist = Mathf.Clamp01(Vector2.Distance(new Vector2(x, y), center) / maxDist);
                pixels[y * size + x] = Color.Lerp(inner, outer, dist);
            }

        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }
}
