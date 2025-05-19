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
        public int rotationx;
        public int rotation;
        public int rotationz;
        public float offsetX;
        public float offsetY;

    }

    [Serializable]
    public class ObjectEntry
    {
        public string name;
        public GameObject prefab;
        public List<ObjectVariation> variations = new List<ObjectVariation>();
        public AudioClip sound1;
        public AudioClip sound2;
        public AudioClip sound3;
        public FeedbackTypeEnum[] feedbackTypes;
        public float hapticForce;

        public List<float> hapticValues;


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
                    rotationx = variation.rotationx,
                    rotation = variation.rotation,
                    rotationz = variation.rotationz,
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
                sound1 = objectEntry.sound1,
                sound2 = objectEntry.sound2,
                sound3 = objectEntry.sound3,
                hapticForce = objectEntry.hapticForce,
                hapticValues = objectEntry.hapticValues
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





