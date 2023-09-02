using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MaterialData", menuName = "Custom/Material Data")]
public class MaterialData : ScriptableObject
{
    [SerializeField]
    private List<Material> materials = new List<Material>();

    public List<Material> Materials => materials;
}
