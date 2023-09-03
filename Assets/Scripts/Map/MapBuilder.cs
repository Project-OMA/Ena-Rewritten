using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using MapObjects;
using MapParser;
public class MapBuilder : MonoBehaviour
{

    [SerializeField] bool disableWalls = false;
    [SerializeField] bool disableFloors = false;
    [SerializeField] bool disableCeilings = false;

    [FormerlySerializedAs("jsonRawFile")]
    [SerializeField] private TextAsset mapFile;


    [SerializeField] private MaterialData floorMaterialData;
    [SerializeField] private MaterialData wallMaterialData;
    [SerializeField] private Material defaultFloorMaterial;
    [SerializeField] private Material defaultWallMaterial;

    [SerializeField] private Mesh floorMesh;
    [SerializeField] private Mesh wallMesh;


    private GameObject floorParent = new GameObject("Floor");
    private GameObject ceilingParent = new GameObject("Ceiling");
    private GameObject wallsParent = new GameObject("Walls");

    private void InstanceFloorTile(string code, int[] startArr, int[] endArr)
    {
        Vector3 start = new Vector3(startArr[0], 0, -startArr[1]);
        Vector3 end = new Vector3(endArr[0] + 1, 0, -endArr[1] - 1);
        Vector3 center = (end - start) / 2;
        Vector3 size = new Vector3(Mathf.Abs(end.x - start.x), 1, Mathf.Abs(end.z - start.z));
        GameObject floorPiece = null;

        // get the material
        Material material = null;
        try
        {
            material = floorMaterialData.GetMaterial(code);
        }
        catch (System.Exception)
        {
            material = defaultFloorMaterial;
        }
        // get the mesh
        Mesh mesh = floorMesh;
        // scale the mesh uvs
        var newUvs = new Vector2[mesh.uv.Length];
        for (var i = 0; i < newUvs.Length; i++)
        {
            newUvs[i] = new Vector2(mesh.uv[i].x * size.x, mesh.uv[i].y * size.z);
        }
        mesh.uv = newUvs;
        // create the floor tile
        floorPiece = GameObject.Instantiate(new GameObject("Floor Tile"));
        floorPiece.transform.position = center;
        floorPiece.transform.localScale = size;
        floorPiece.GetComponent<MeshRenderer>().material = material;
        floorPiece.GetComponent<MeshFilter>().mesh = mesh;
        floorPiece.transform.parent = floorParent.transform;
    }

    private void InstanceWallTile(string code, int[] startArr, int[] endArr)
    { }

    private void InstanceCeilingTile(string code, int[] startArr, int[] endArr)
    { }


    void BuildMap(Map map)
    {
        List<Wall> walls = map.layers.walls;
        List<Floor> floors = map.layers.floors;
        List<DoorAndWindow> door_and_windows = map.layers.door_and_windows;
        List<Furniture> furniture = map.layers.furniture;
        List<Utensil> utensils = map.layers.utensils;
        List<Electronic> eletronics = map.layers.eletronics;
        List<Goal> goals = map.layers.goals;
        List<Person> persons = map.layers.persons;

        // Build the floors
        if (!disableFloors)
        {
            foreach (Floor floor in floors)
            {
                InstanceFloorTile(floor.type, floor.start, floor.end);
            }
        }
    }


    void Awake()
    {
        // Parse the map file
        IMapParser mapParser = new MapParserJSON(mapFile.text);
        Map map = mapParser.ParseMap();

        // Build the map
        BuildMap(map);
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
