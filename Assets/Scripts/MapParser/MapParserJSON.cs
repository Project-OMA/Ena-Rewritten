using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapObjects;
using JsonUtility = UnityEngine.JsonUtility;

namespace MapParser
{
    public class MapParserJSON : IMapParser
    {
        public string jsonData;

        public MapParserJSON(string jsonData)
        {
            this.jsonData = jsonData;
        }

        //  Create a Map object from the JSON string
        private Map CreateFromJSON(string jsonString)
        {

            var data = JsonUtility.FromJson<Map>(jsonString);

            // If ceiling is not defined, set it to the same as floor
            var ceilings = data.layers.ceilings;
            if (!jsonString.Contains("ceilings"))
            {
                Debug.Log("Ceiling not defined, setting it to the same as floor");
                ceilings = new List<Ceiling>();
                var floors = data.layers.floors;
                for (int i = 0; i < floors.Count; i++)
                {
                    Ceiling ceiling = new()
                    {
                        type = "0.0", // default ceiling type
                        start = floors[i].start,
                        end = floors[i].end
                    };
                    ceilings.Add(ceiling);
                }
                data.layers.ceilings = ceilings;
            }

            return data;
        }

        public Map ParseMap()
        {
            Debug.Log("Parsing map from JSON");
            return CreateFromJSON(jsonData);
        }
    }
}
