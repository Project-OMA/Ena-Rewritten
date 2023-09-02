using System.Collections.Generic;
using UnityEngine;

public class TextureManager
{
    // Singleton
    private static TextureManager instance;
    public static TextureManager GetInstance()
    {
        if (instance == null)
        {
            instance = new TextureManager();
        }
        return instance;
    }

    // Private constructor
    private TextureManager()
    {
    }

    private Dictionary<string, Material> materialMap = new Dictionary<string, Material>();
    private MaterialData materialData; // Reference to the MaterialData ScriptableObject

    // Initialize the TextureManager with the MaterialData
    public void Initialize(MaterialData data)
    {
        materialData = data;
        foreach (var material in data.Materials)
        {
            materialMap[material.name] = material;
        }
    }

    public Material GetMaterial(string name)
    {
        if (materialMap.ContainsKey(name))
        {
            return materialMap[name];
        }
        else
        {
            throw new System.Exception("Material " + name + " not found");
        }
    }

    public Material GetDefaultMaterial()
    {
        return materialData.Materials[0]; // Example: Return the first material in the list as the default
    }

    public Material RegisterMaterial(string name, Material material)
    {
        if (materialMap.ContainsKey(name))
        {
            throw new System.Exception("Material " + name + " already exists");
        }
        else
        {
            materialMap.Add(name, material);
            materialData.Materials.Add(material); // Add the material to the MaterialData ScriptableObject
            return material;
        }
    }
}
