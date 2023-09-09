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
    public class ObjectEntry
    {
        public string id;
        public string name;
        public GameObject prefab;

        public int rotation;
    }

    public GameObject GetObject(string id)
    {
        if (Objects.Exists(x => x.id == id))
        {
            return Objects.Find(x => x.id == id).prefab;
        }
        else
        {
            throw new System.Exception("Object " + id + " not found");
        }
    }

    public GameObject GetDefaultObject()
    {
        if (Objects.Count > 0)
        {
            return Objects[0].prefab; // Return the first object in the list as the default
        }
        else
        {
            throw new System.Exception("No objects available in ObjectData");
        }
    }

    public GameObject RegisterObject(string id, string name, GameObject prefab)
    {
        if (objectMap.ContainsKey(id))
        {
            throw new System.Exception("Object " + id + " already registered");
        }
        else
        {
            objectMap[id] = prefab;
            Objects.Add(new ObjectEntry { id = id, name = name, prefab = prefab });
            return prefab;
        }
    }

    public void UnregisterObject(string id)
    {
        if (objectMap.ContainsKey(id))
        {
            objectMap.Remove(id);
            Objects.RemoveAll(x => x.id == id);
        }
        else
        {
            throw new System.Exception("Object " + id + " not registered");
        }
    }
}
