using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapObjects;

namespace MapParser
{
    public class MapParserXML : IMapParser
    {
        public string xmlData;

        public MapParserXML(string xmlData)
        {
            this.xmlData = xmlData;
        }

        public Map ParseMap()
        {
            Debug.Log("Parsing map from XML");
            return new Map();
        }
    }
}
