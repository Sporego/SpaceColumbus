using System;
using System.Text;
using System.Collections.Generic;
using System.Xml;

namespace Utilities.XmlReader
{
    public struct XmlNodeData
    {
        public string Name;
        public string Data;
        public List<XmlNodeData> Children;

        public XmlNodeData(string Name, string Data, List<XmlNodeData> children)
        {
            this.Name = Name;
            this.Data = Data;
            this.Children = children;
        }
    }

    public class XmlReader
    {
        // TODO: add maximum number of currently open XmlDocuments, order these by open time, i.e. cache

        private static Dictionary<string, XmlDocument> openDocs = new Dictionary<string, XmlDocument>();

        // all paths passed to this class are relative to current directory
        // ex: path "/assets/xml_defs/stats.xml"
        private static string CurDir = System.IO.Directory.GetCurrentDirectory();

        #region NonStatic
        private XmlDocument doc;

        // this class helps with accessing the static documents
        public XmlReader(string path)
        {
            this.doc = GetXmlDoc(path);
        }

        public bool hasField(string field) { return hasField(this.doc, field); }
        public bool hasField(List<string> fields) { return hasField(this.doc, fields); }
        public float getFloat(string field) { return getFloat(this.doc, field); }
        public float getFloat(List<string> fields) { return getFloat(this.doc, fields); }
        public string getString(string field) { return getString(this.doc, field); }
        public string getString(List<string> fields) { return getString(this.doc, fields); }
        public List<string> getStrings(string field) { return getStrings(this.doc, field); }
        public List<string> getStrings(List<string> fields) { return getStrings(this.doc, fields); }
        public List<string> getChildren(string field) { return getChildren(this.doc, field); }
        public List<string> getChildren(List<string> fields) { return getChildren(this.doc, fields); }
        #endregion NonStatic

        public static bool hasField(XmlDocument doc, List<string> fields)
        {
            return hasField(doc, GetFieldPathFromStringList(fields));
        }
        public static bool hasField(XmlDocument doc, string fieldPath)
        {
            XmlNode node = doc.DocumentElement.SelectSingleNode("/" + fieldPath);
            return node != null;
        }

        public static float getFloat(XmlDocument doc, List<string> fields)
        {
            return getFloat(doc, GetFieldPathFromStringList(fields));
        }

        public static float getFloat(XmlDocument doc, string fieldPath)
        {
            var s = getString(doc, fieldPath);
            return float.Parse(s);
        }

        public static string getString(XmlDocument doc, List<string> fields)
        {
            return getString(doc, GetFieldPathFromStringList(fields));
        }

        public static string getString(XmlDocument doc, string fieldPath)
        {
            XmlNode node = doc.DocumentElement.SelectSingleNode("/" + fieldPath);
            if (node != null)
            {
                return node.InnerText;
            }
            else
                return "";
        }

        public static List<string> getStrings(XmlDocument doc, List<string> fields)
        {
            return getStrings(doc, GetFieldPathFromStringList(fields));
        }

        public static List<string> getStrings(XmlDocument doc, string fieldPath)
        {
            XmlNodeList nodes = doc.DocumentElement.SelectNodes("/" + fieldPath);
            return (nodes == null) ? null : RecursiveNodeToString(nodes);
        }

        public static List<string> getChildren(XmlDocument doc, List<string> fields)
        {
            return getChildren(doc, GetFieldPathFromStringList(fields));
        }

        public static List<string> getChildren(XmlDocument doc, string fieldPath)
        {
            List<string> strings = new List<string>();
            XmlNodeList nodes = doc.DocumentElement.SelectNodes("/" + fieldPath);
            if (nodes != null)
            {
                foreach (XmlNode node in nodes)
                    foreach (XmlNode childNode in node.ChildNodes)
                        strings.Add(childNode.Name);
                
            }
            return strings;
        }

        public static List<String> RecursiveNodeToString(XmlNodeList nodes, string name = "")
        {
            List<string> strings = new List<string>();
            foreach (XmlNode node in nodes)
                strings.AddRange(RecursiveNodeToString(node, name));
            return strings;
        }

        public static List<String> RecursiveNodeToString(XmlNode node, string name="")
        {
            List<string> strings = new List<string>();
            if (node.HasChildNodes)
                foreach (XmlNode child in node.ChildNodes)
                    foreach (var s in RecursiveNodeToString(child))
                        strings.Add(s);
            else if (name == "" || node.Name == name)
                strings.Add(node.InnerText);
            return strings;
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

        private static XmlDocument GetXmlDoc(string path)
        {
            if (openDocs.ContainsKey(path))
                return openDocs[path];
            else
                return AddNewXmlDoc(path);
        }

        private static string GetFieldPathFromStringList(List<string> fields)
        {
            // TODO: verify '/' at the end
            StringBuilder fieldPath = new StringBuilder();
            for (int i = 0; i < fields.Count; i++)
            {
                fieldPath.Append(fields[i]);
                if (i < fields.Count - 1)
                    fieldPath.Append("/");
            }
            return fieldPath.ToString();
        }

        private static XmlDocument ReadXmlDocument(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(CurDir + "/" + path);
            return doc;
        }
    }
}
