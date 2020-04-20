using UnityEngine;

using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Utilities.Misc
{
    public static class Lines
    {
        public struct LineSegment
        {
            public Vector2 p;
            public Vector2 q;

            public LineSegment(Vector2 p, Vector2 q)
            {
                this.p = p;
                this.q = q;
            }
        }

        // Given three colinear points p, q, r, the function checks if 
        // point q lies on line segment 'pr' 
        public static bool OnSegment(Vector2 p, Vector2 q, Vector2 r)
        {
            if (q.x <= Mathf.Max(p.x, r.x) && q.x >= Mathf.Min(p.x, r.x) &&
                q.y <= Mathf.Max(p.y, r.y) && q.y >= Mathf.Max(p.y, r.y))
                return true;

            return false;
        }

        // To find orientation of ordered triplet (p, q, r). 
        // The function returns following values 
        // 0 --> p, q and r are colinear 
        // 1 --> Clockwise 
        // 2 --> Counterclockwise 
        public static int Orientation(Vector2 p, Vector2 q, Vector2 r)
        {
            // See https://www.geeksforgeeks.org/orientation-3-ordered-points/ 
            // for details of below formula. 
            float val = (q.y - p.y) * (r.x - q.x) -
                      (q.x - p.x) * (r.y - q.y);

            if (val == 0) return 0;  // colinear 

            return (val > 0) ? 1 : 2; // clock or counterclock wise 
        }

        public static bool DoIntersect(LineSegment s1, LineSegment s2) { return DoIntersect(s1.p, s1.q, s2.p, s2.q); }

        // The main function that returns true if line segment 'p1q1' 
        // and 'p2q2' intersect. 
        public static bool DoIntersect(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2)
        {
            // Find the four orientations needed for general and 
            // special cases 
            int o1 = Orientation(p1, q1, p2);
            int o2 = Orientation(p1, q1, q2);
            int o3 = Orientation(p2, q2, p1);
            int o4 = Orientation(p2, q2, q1);

            // General case 
            if (o1 != o2 && o3 != o4)
                return true;

            // Special Cases 
            // p1, q1 and p2 are colinear and p2 lies on segment p1q1 
            if (o1 == 0 && OnSegment(p1, p2, q1)) return true;

            // p1, q1 and q2 are colinear and q2 lies on segment p1q1 
            if (o2 == 0 && OnSegment(p1, q2, q1)) return true;

            // p2, q2 and p1 are colinear and p1 lies on segment p2q2 
            if (o3 == 0 && OnSegment(p2, p1, q2)) return true;

            // p2, q2 and q1 are colinear and q1 lies on segment p2q2 
            if (o4 == 0 && OnSegment(p2, q1, q2)) return true;

            return false; // Doesn't fall in any of the above cases 
        }
    }

    public static class Encrypt
    {
        public static string sha256(string str)
        {
            SHA256Managed crypt = new SHA256Managed();
            StringBuilder hash = new StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(str), 0, Encoding.UTF8.GetByteCount(str));
            foreach (byte bit in crypto)
            {
                hash.Append(bit.ToString("x2"));
            }
            return hash.ToString().ToLower();
        }
    }

    public static class Constants
    {
        public const float EPSILON = 1e-9f;
    }

    public static class LoggerDebug
    {
        private delegate void Log();

        public static void LogS(params object[] list)
        {
            Debug.Log(Tools.BuildString(list));
        }

        public static void LogT(string tag, params object[] list)
        {
            Debug.Log(tag + ": " + Tools.BuildString(list));
        }

        public static void LogE(params object[] list)
        {
            Debug.LogError(Tools.BuildString(list));
        }

        public static void LogET(string tag, params object[] list)
        {
            Debug.LogError(tag + ": " + Tools.BuildString(list));
        }

        public static void LogW(params object[] list)
        {
            Debug.LogWarning(Tools.BuildString(list));
        }

        public static void LogWT(string tag, params object[] list)
        {
            Debug.LogWarning(tag + ": " + Tools.BuildString(list));
        }
    }

    public static class Samplers
    {
        // returns the array index of a randomly picked sample given the individual pdfs of each sample
        public static int SampleFromPdf(float sample, List<float> pdfs)
        {
            if (pdfs.Count < 2)
                LoggerDebug.LogT("ProbabilitySampler", "Trying to sample from a PDF with less than 2 items.");

            float cdfMax = pdfs.Sum() - Constants.EPSILON; // subtract epsilon (small nudge) to ensure that cdf=1 is reachable
            float cdf = 0f;
            for (int i = 0; i < pdfs.Count; i++)
            {
                cdf += pdfs[i] / cdfMax;
                if (sample <= cdf)
                    return i;
            }

            return pdfs.Count - 1;
        }

        // returns min and max array indices of the picked samples, given individual pdfs and a tolerance parameter
        public static Vector2Int SampleFromPdf(float sample, List<float> pdfs, float tolerance)
        {
            int min = SampleFromPdf(sample - tolerance, pdfs);
            int max = SampleFromPdf(sample + tolerance, pdfs);
            return new Vector2Int(min, max);
        }


        public static Vector3 sampleRandomCosineHemisphere(float u, float v)
        {
            return sampleRandomCosineHemisphere(new Vector2(u, v));
        }

        public static Vector3 sampleRandomCosineHemisphere(Vector2 uv)
        {
            uv = 2f * uv - new Vector2(1, 1);

            float theta, r;
            if (Mathf.Abs(uv.x) > Mathf.Abs(uv.y))
            {
                r = uv.x;
                theta = Mathf.PI / 4f * uv.y / uv.x;
            }
            else
            {
                r = uv.y;
                theta = Mathf.PI / 2f - Mathf.PI / 4f * uv.x / uv.y;
            }

            uv = r * new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));

            float z = Mathf.Sqrt(1f - uv.x * uv.x - uv.y * uv.y);
            return new Vector3(uv.x, z, uv.y);
        }
    }

    public static class Tools
    {
        public static string BuildString(params object[] list)
        {
            StringBuilder sb = new StringBuilder();

            for(int i = 0; i < list.Length; i++) 
            {
                sb.Append(Convert.ToString(list[i]));
                if (i < list.Length - 1) // append after all except last element
                    sb.Append(" ");
            }

            return sb.ToString();
        }

        public static Color hexToColor(string hex)
        {
            Color c = new Color();
            ColorUtility.TryParseHtmlString(hex, out c);
            return c;
        }

        public static float[] computeMinMaxAvg(float[,] values)
        {
            // get average and max elevation values
            float avg = 0, max = 0, min = float.MaxValue;
            for (int i = 0; i < values.GetLength(0); i++)
            {
                for (int j = 0; j < values.GetLength(0); j++)
                {
                    avg += values[i, j];
                    if ((values[i, j] > max))
                        max = values[i, j];
                    if ((values[i, j] < min))
                        min = values[i, j];
                }
            }

            avg /= values.GetLength(0) * values.GetLength(0); // since elevations is 2d array nxn
            Debug.Log("Pre min/max/avg: " + min + "/" + max + "/" + avg);

            return new float[] { min, max, avg };
        }

        public static void normalize(float[,] values, bool maxOnly = false, bool rescaleSmallMax = true)
        {
            float[] minMaxAvg = computeMinMaxAvg(values);
            float min = minMaxAvg[0];
            float max = minMaxAvg[1];
            float avg = minMaxAvg[2];

            // configuration modifiers
            min = maxOnly ? 0 : min;
            max = !rescaleSmallMax && max < 1f ? 1f : max;

            float adjustment = 1f;
            if (maxOnly && max > 1e-4f)
                adjustment = 1f / max;
            else if (max - min > 1e-4f)
                adjustment = 1f / (max - min);

            if (adjustment != 1f)
                for (int i = 0; i < values.GetLength(0); i++)
                {
                    for (int j = 0; j < values.GetLength(0); j++)
                    {
                        values[i, j] = (Mathf.Abs(values[i, j] - min) * adjustment);
                    }
                }

            avg = 0;
            max = 0;
            min = float.MaxValue;
            for (int i = 0; i < values.GetLength(0); i++)
            {
                for (int j = 0; j < values.GetLength(0); j++)
                {
                    avg += values[i, j];
                    if ((values[i, j] > max))
                        max = values[i, j];
                    if ((values[i, j] < min))
                        min = values[i, j];
                }
            }
            Debug.Log("Post min/max/avg: " + min + "/" + max + "/" + avg / (values.GetLength(0) * values.GetLength(0)));
        }

        public static float[,] mergeArrays(float[,] a, float[,] b, float weightA, float weightB, bool overwrite = false)
        {
            if (weightA <= 0 && weightB <= 0)
            {
                weightA = 0.5f;
                weightB = 0.5f;
            }

            weightA = weightA / (weightA + weightB);
            weightB = weightB / (weightA + weightB);

            // works with arrays of different size
            bool choice = a.GetLength(0) > b.GetLength(0);

            float[,] dst;
            if (overwrite)
            {
                dst = a;
            }
            else
            {
                dst = (choice) ? new float[a.GetLength(0), a.GetLength(0)] : new float[b.GetLength(0), b.GetLength(0)];
            }

            double ratio = (double)a.GetLength(0) / b.GetLength(0);
            for (int i = 0; i < dst.GetLength(0); i++)
            {
                for (int j = 0; j < dst.GetLength(0); j++)
                {
                    // sum weighted values
                    if (choice)
                    {
                        dst[i, j] = weightA * a[i, j] + weightB * b[(int)(i / ratio), (int)(j / ratio)];
                    }
                    else
                    {
                        dst[i, j] = weightA * a[(int)(i * ratio), (int)(j * ratio)] + weightB * b[i, j];
                    }
                    // rescale the values back
                    dst[i, j] /= (weightA + weightB);
                }
            }

            return dst;
        }
    }
}