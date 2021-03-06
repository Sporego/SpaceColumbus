﻿using UnityEngine;

using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Utilities.Misc
{
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

    public static class Slicable
    {
        public static T[] Slice<T>(this T[] source, int start, int end)
        {
            // Handles negative ends.
            if (end < 0)
            {
                end = source.Length + end;
            }
            int len = end - start;

            // Return new array.
            T[] res = new T[len];
            for (int i = 0; i < len; i++)
            {
                res[i] = source[i + start];
            }
            return res;
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