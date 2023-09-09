using System.IO;
using System.Xml;
using UnityEngine;


namespace MapParser
{

    public class XMLParser
    {

        XmlDocument document;

        public XMLParser(string xmlData)
        {
            document = XMLParser.DocumentFrom(xmlData);
        }

        public XmlNode[] FetchAllItems(string path)
        {
            XmlNodeList list = document.SelectNodes(path);
            XmlNode[] array = new XmlNode[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                array[i] = list.Item(i);
            }

            return array;
        }
        public XmlNode Fetch(string path)
        {
            return document.SelectSingleNode(path);
        }

        public static XMLParser Create(string xmlData)
        {
            var parser = new XMLParser(xmlData);
            return parser;
        }

        public static XmlDocument DocumentFrom(string xmlData)
        {
            xmlData = xmlData.Replace("xmlns=\"http://www.w3.org/1999/xhtml\"", "");
            var document = new XmlDocument();
            document.Load(new StringReader(xmlData));
            return document;
        }
    }
}
