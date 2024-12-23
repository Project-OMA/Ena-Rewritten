using System;
using System.Collections.Generic;
using UnityEngine;
using MapObjects;
using System.Linq;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 1)]

public class PlayerData : ScriptableObject
{

    [SerializeField]
    public Dictionary<string, GameObject> PlayerMap = new Dictionary<string, GameObject>();

    [SerializeField]
    public List<PlayerEntry> Players = new List<PlayerEntry>();


    [Serializable]
    public class PlayerVariation
    {
        public string id;
        public int rotationx;
        public int rotation;
        public int rotationz;
        public float offsetX;
        public float offsetY;

    }

    [Serializable]
    public class PlayerEntry
    {
        public string name;
        public GameObject prefab;
        public List<PlayerVariation> variations = new List<PlayerVariation>();

    }

    public ObjectPrefab GetPlayer(string id)
    {
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].variations.Exists(x => x.id == id))
            {
                PlayerVariation variation = Players[i].variations.Find(x => x.id == id);
                return new ObjectPrefab
                {
                    name = Players[i].name,
                    prefab = Players[i].prefab,
                    rotationx = variation.rotationx,
                    rotation = variation.rotation,
                    rotationz = variation.rotationz,
                    offsetX = variation.offsetX,
                    offsetY = variation.offsetY
                };
            }
        }
        throw new System.Exception("Player " + id + " not found");
    }


    private bool TryGetPlayerEntry(string name, out PlayerEntry PlayerEntry)
    {
        
            
        PlayerEntry = Players.FirstOrDefault(x => x.name == name);
        return PlayerEntry != null;
        
        
    }        
    

}





