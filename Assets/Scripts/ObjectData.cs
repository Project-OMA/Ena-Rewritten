using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectData", menuName = "ScriptableObjects/ObjectData", order = 1)]

public class ObjectData : ScriptableObject
{

    [SerializeField]
    public Dictionary<string, GameObject> objectMap = new Dictionary<string, GameObject>();

    [SerializeField]
    public List<ObjectEntry> Objects = new List<ObjectEntry>();


    [System.Serializable]
    public class ObjectVariation
    {
        public string id;
        public int rotation;
        public int offsetX;
        public int offsetY;
    }

    [System.Serializable]
    public class ObjectEntry
    {
        public string name;
        public GameObject prefab;
        public List<ObjectVariation> variations = new List<ObjectVariation>();
    }

    [System.Serializable]
    public class ObjectPrefab
    {
        // Used as a return type for GetObject
        public GameObject prefab;
        public int rotation;
        public int offsetX;
        public int offsetY;
    }

    public ObjectPrefab GetObject(string id)
    {
        for (int i = 0; i < Objects.Count; i++)
        {
            if (Objects[i].variations.Exists(x => x.id == id))
            {
                ObjectVariation variation = Objects[i].variations.Find(x => x.id == id);
                return new ObjectPrefab
                {
                    prefab = Objects[i].prefab,
                    rotation = variation.rotation,
                    offsetX = variation.offsetX,
                    offsetY = variation.offsetY
                };
            }
        }
        throw new System.Exception("Object " + id + " not found");
    }
}
