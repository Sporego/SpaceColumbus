using UnityEngine;
using System.Xml;
using System.Collections.Generic;

namespace Utilities.Misc
{
    public static class Samplers
    {
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

    public static class StatsXMLreader
    {
        private static Dictionary<string, XmlDocument> openDocs;

        private static string CurDir = System.IO.Directory.GetCurrentDirectory();

        // path is relative to current directory
        // ex: path "/assets/xml_defs/stats.xml"
        public static XmlDocument GetXmlDoc(string path)
        {
            if (openDocs.ContainsKey(path))
                return openDocs[path];
            else
                return AddNewXmlDoc(path);
        }

        public static string GetFieldPathFromStringList(List<string> fields)
        {
            string fieldPath = "";
            foreach (var field in fields)
            {
                fieldPath += field;
            }
            return fieldPath;
        }

        public static List<string> getParametersFromXML(string path, List<string> fields)
        {
            return getParametersFromXML(GetXmlDoc(path), GetFieldPathFromStringList(fields));
        }

        public static List<string> getParametersFromXML(string path, string fieldPath)
        {
            return getParametersFromXML(GetXmlDoc(path), fieldPath);
        }

        public static List<string> getParametersFromXML(XmlDocument doc, List<string> fields)
        {
            return getParametersFromXML(doc, GetFieldPathFromStringList(fields));
        }

        public static List<string> getParametersFromXML(XmlDocument doc, string fieldPath)
        {
            List<string> strings;
            XmlNodeList nodes = doc.DocumentElement.SelectNodes("/" + fieldPath);
            if (nodes != null)
            {
                strings = new List<string>();
                foreach (XmlNode node in nodes)
                    strings.Add(node.InnerText);
                return strings;
            }
            else
                return null;
        }

        public static XmlDocument ReadXmlDocument(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(CurDir + "/" + path);
            return doc;
        }

        public static XmlDocument AddNewXmlDoc(string path)
        {
            XmlDocument doc = ReadXmlDocument(path);
            openDocs.Add(path, doc);
            return doc;
        }

        public static void ReloadOpenDocs()
        {
            foreach (var path in openDocs.Keys)
            {
                openDocs[path] = ReadXmlDocument(path);
            }
        }

        public static void ClearOpenDocs()
        {
            openDocs = new Dictionary<string, XmlDocument>();
        }
    }
}