using UnityEngine;

namespace Ambient
{
    static class Figures
    {
        static Texture2D fig;

        public static Texture2D Silhouette()
        {
            if (fig == null)
            {
                fig = Build(120, 240);
            }
            return fig;
        }

        static float Shape(float ny, int w)
        {
            const float headC = 0.11f;
            const float headRy = 0.085f;
            float headRx = w * 0.19f;

            if (ny < 0.21f)
            {
                float t = (ny - headC) / headRy;
                if (Mathf.Abs(t) < 1f)
                {
                    return headRx * Mathf.Sqrt(1f - t * t);
                }
                return w * 0.055f;
            }
            if (ny < 0.28f)
            {
                float t = Mathf.InverseLerp(0.21f, 0.28f, ny);
                return Mathf.Lerp(w * 0.07f, w * 0.33f, t);
            }
            float t2 = Mathf.InverseLerp(0.28f, 1f, ny);
            float bw = Mathf.Lerp(w * 0.33f, w * 0.13f, t2);
            return bw * (1f + 0.05f * Mathf.Sin(ny * 20f));
        }

        static float[] Blur(float[] src, int w, int h)
        {
            var dst = new float[w * h];
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float sum = 0f;
                    int n = 0;
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            int xx = x + dx;
                            int yy = y + dy;
                            if (xx < 0 || yy < 0 || xx >= w || yy >= h)
                            {
                                continue;
                            }
                            sum += src[yy * w + xx];
                            n++;
                        }
                    }
                    dst[y * w + x] = sum / n;
                }
            }
            return dst;
        }

        static Texture2D Build(int w, int h)
        {
            var a = new float[w * h];
            float cx = w * 0.5f;
            for (int y = 0; y < h; y++)
            {
                float ny = (float)y / h;
                float half = Shape(ny, w);
                for (int x = 0; x < w; x++)
                {
                    if (Mathf.Abs(x - cx) <= half)
                    {
                        a[y * w + x] = 1f;
                    }
                }
            }
            a = Blur(a, w, h);
            a = Blur(a, w, h);
            a = Blur(a, w, h);
            for (int y = 0; y < h; y++)
            {
                float ny = (float)y / h;
                float fade = ny > 0.82f ? Mathf.Clamp01(1f - (ny - 0.82f) / 0.18f) : 1f;
                for (int x = 0; x < w; x++)
                {
                    a[y * w + x] *= fade;
                }
            }

            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.hideFlags = HideFlags.HideAndDontSave;
            float ex = w * 0.075f;
            float ey = h * 0.105f;
            float er = w * 0.026f;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float av = a[y * w + x];
                    float r = 0.015f, g = 0.01f, b = 0.02f;
                    float dxl = x - (cx - ex);
                    float dxr = x - (cx + ex);
                    float dy = y - ey;
                    if (dxl * dxl + dy * dy <= er * er || dxr * dxr + dy * dy <= er * er)
                    {
                        r = 0.55f;
                        g = 0.05f;
                        b = 0.05f;
                        av = Mathf.Max(av, 0.85f);
                    }
                    tex.SetPixel(x, h - 1 - y, new Color(r, g, b, av));
                }
            }
            tex.Apply();
            return tex;
        }
    }
}
