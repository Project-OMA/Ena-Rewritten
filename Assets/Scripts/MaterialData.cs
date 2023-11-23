using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MaterialData", menuName = "ScriptableObjects/MaterialData", order = 1)]
public class MaterialData : ScriptableObject
{
    [SerializeField]
    private Dictionary<string, Material> materialMap = new Dictionary<string, Material>();

    [SerializeField]
    private List<MaterialEntry> materials = new List<MaterialEntry>();

    [Serializable]
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
        if (TryGetMaterialEntry(id, out var materialEntry))
        {
            return materialEntry.material;
        }
        throw new ArgumentException($"Material {id} not found");
    }

    public FeedbackSettings GetFeedbackSettings(string id)
    {
        if (TryGetMaterialEntry(id, out var materialEntry))
        {
            return new FeedbackSettings
            {
                feedbackTypes = materialEntry.feedbackTypes,
                sound = materialEntry.sound,
                hapticForce = materialEntry.hapticForce
            };
        }
        throw new ArgumentException($"Material {id} not found");
    }

    public bool DoesMaterialUseGlobalUV(string id)
    {
        if (TryGetMaterialEntry(id, out var materialEntry))
        {
            return materialEntry.useGlobalUV;
        }
        throw new ArgumentException($"Material {id} not found");
    }

    public Vector2 GetMaterialScale(string id)
    {
        if (TryGetMaterialEntry(id, out var materialEntry))
        {
            return materialEntry.scale;
        }
        throw new ArgumentException($"Material {id} not found");
    }

    public Material GetDefaultMaterial()
    {
        if (materials.Count > 0)
        {
            return materials[0].material;
        }
        throw new InvalidOperationException("No materials available in MaterialData");
    }

    public Material RegisterMaterial(string id, string name, Material material)
    {
        if (materialMap.ContainsKey(id))
        {
            throw new InvalidOperationException($"Material {id} already registered");
        }
        materialMap[id] = material;
        materials.Add(new MaterialEntry { id = id, name = name, material = material });
        return material;
    }

    public void UnregisterMaterial(string id)
    {
        if (materialMap.ContainsKey(id))
        {
            materialMap.Remove(id);
            materials.RemoveAll(x => x.id == id);
        }
        else
        {
            throw new ArgumentException($"Material {id} not registered");
        }
    }

    private bool TryGetMaterialEntry(string id, out MaterialEntry materialEntry)
    {
        materialEntry = materials.FirstOrDefault(x => x.id == id);
        return materialEntry != null;
    }
}
