using System;
using System.Collections.Generic;
using UnityEngine;
using MapObjects;
using System.Linq;

[CreateAssetMenu(fileName = "ObjectData", menuName = "ScriptableObjects/ObjectData", order = 1)]

public class ObjectData : ScriptableObject
{

    [SerializeField]
    public Dictionary<string, GameObject> objectMap = new Dictionary<string, GameObject>();

    [SerializeField]
    public List<ObjectEntry> Objects = new List<ObjectEntry>();


    [Serializable]
    public class ObjectVariation
    {
        public string id;
        public int rotation;
        public int offsetX;
        public int offsetY;

    }

    [Serializable]
    public class ObjectEntry
    {
        public string name;
        public GameObject prefab;
        public List<ObjectVariation> variations = new List<ObjectVariation>();
        public AudioClip sound;
        public FeedbackTypeEnum[] feedbackTypes;
        public float hapticForce;


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
                    name = Objects[i].name,
                    prefab = Objects[i].prefab,
                    rotation = variation.rotation,
                    offsetX = variation.offsetX,
                    offsetY = variation.offsetY

                };
            }
        }
        throw new System.Exception("Object " + id + " not found");
    }


    public FeedbackSettings GetFeedbackSettings(string name)
    {
        
        

        if (TryGetObjectEntry(name, out var objectEntry))
        {
            return new FeedbackSettings
            {
                feedbackTypes = objectEntry.feedbackTypes,
                sound = objectEntry.sound,
                hapticForce = objectEntry.hapticForce
            };
        }
        throw new ArgumentException($"Object {name} not found");
    }

    private bool TryGetObjectEntry(string name, out ObjectEntry objectEntry)
    {
        
            
        objectEntry = Objects.FirstOrDefault(x => x.name == name);
        return objectEntry != null;
        
        
    }        
    

}





