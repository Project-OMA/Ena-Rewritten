using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MaterialData", menuName = "ScriptableObjects/MaterialData", order = 1)]
public class MaterialData : ScriptableObject
{


    [SerializeField]
    public Dictionary<string,Material> materialMap = new Dictionary<string,Material>();

    [SerializeField]
    public List<MaterialEntry> Materials = new List<MaterialEntry>();

    [System.Serializable]
    public class MaterialEntry
    {
        public string id;
        public string name;
        public Material material;

        public bool useGlobalUV = false;

        public Vector2 scale = Vector2.one; 

        public AudioClip sound;
        public FeedbackTypeEnum[] feedbackTypes;
        public float hapticForce;
    }

    public Material GetMaterial(string id)
    {
        if (Materials.Exists(x => x.id == id))
        {
            return Materials.Find(x => x.id == id).material;
        }
        else
        {
            throw new System.Exception("Material " + id + " not found");
        }
    }

    public FeedbackSettings GetFeedbackSettings(string id)
    {
        if (Materials.Exists(x => x.id == id))
        {
            var material = Materials.Find(x => x.id == id);
            return new FeedbackSettings
            {
                feedbackTypes = material.feedbackTypes,
                sound = material.sound,
                hapticForce = material.hapticForce
            };
        }
        else
        {
            throw new System.Exception("Material " + id + " not found");
        }
    }

    public bool DoesMaterialUsesGlobalUV(string id)
    {
        if (Materials.Exists(x => x.id == id))
        {
            return Materials.Find(x => x.id == id).useGlobalUV;
        }
        else
        {
            throw new System.Exception("Material " + id + " not found");
        }
    }

    public Vector2 GetMaterialScale(string id)
    {
        if (Materials.Exists(x => x.id == id))
        {
            return Materials.Find(x => x.id == id).scale;
        }
        else
        {
            throw new System.Exception("Material " + id + " not found");
        }
    }

    public Material GetDefaultMaterial()
    {
        if (Materials.Count > 0)
        {
            return Materials[0].material; // Example: Return the first material in the list as the default
        }
        else
        {
            throw new System.Exception("No materials available in MaterialData");
        }
    }

    public Material RegisterMaterial(string id,string name, Material material)
    {
        if (materialMap.ContainsKey(id))
        {
            throw new System.Exception("Material " + id + " already registered");
        }
        else
        {
            materialMap[id] = material;
            Materials.Add(new MaterialEntry { id = id,name = name, material = material });
            return material;
        }
    }

    public void UnregisterMaterial(string id)
    {
        if (materialMap.ContainsKey(id))
        {
            materialMap.Remove(id);
            Materials.RemoveAll(x => x.id == id);
        }
        else
        {
            throw new System.Exception("Material " + id + " not registered");
        }
    }
}
