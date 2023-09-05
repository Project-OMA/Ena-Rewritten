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

    private Mesh floorMesh;
    private Mesh wallMesh;


    private GameObject floorParent;
    private GameObject ceilingParent;
    private GameObject wallsParent;

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
            Debug.LogError("Material " + code + " not found");
            material = defaultFloorMaterial;
        }



        // make a copy of the floor mesh
        Mesh mesh = new Mesh();
        mesh.vertices = floorMesh.vertices;
        mesh.triangles = floorMesh.triangles;
        mesh.uv = floorMesh.uv;
        // scale the mesh uvs
        var newUvs = new Vector2[mesh.uv.Length];
        for (var i = 0; i < newUvs.Length; i++)
        {
            newUvs[i] = new Vector2(mesh.uv[i].x * size.x, mesh.uv[i].y * size.z);
        }
        mesh.uv = newUvs;
        // fix the mesh normals
        mesh.RecalculateNormals();


        // create the floor tile
        floorPiece = new GameObject("Floor:" + startArr[0] + "_" + startArr[1]+"_"+endArr[0]+"_"+endArr[1]);
        floorPiece.transform.position = start + center;
        floorPiece.transform.rotation = Quaternion.identity;
        floorPiece.transform.localScale = size;
        floorPiece.transform.parent = floorParent.transform;
        //add mesh renderer and filter
        floorPiece.AddComponent<MeshRenderer>();
        floorPiece.AddComponent<MeshFilter>();
        floorPiece.AddComponent<MeshCollider>();


        floorPiece.GetComponent<MeshRenderer>().material = material;
        floorPiece.GetComponent<MeshFilter>().mesh = mesh;
        floorPiece.GetComponent<MeshCollider>().sharedMesh = mesh; 
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


    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        // Floor mesh
        this.floorMesh = new Mesh();
        this.floorMesh.vertices = new Vector3[] { new Vector3(-0.5f, 0, -0.5f), new Vector3(-0.5f, 0, 0.5f), new Vector3(0.5f, 0, 0.5f), new Vector3(0.5f, 0, -0.5f) };
        this.floorMesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        this.floorMesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) };
        // Wall mesh, its the same as the but vertical
        this.wallMesh = new Mesh();
        this.wallMesh.vertices = new Vector3[] { new Vector3(-0.5f, 0, -0.5f), new Vector3(-0.5f, 0, 0.5f), new Vector3(-0.5f, 3, -0.5f), new Vector3(-0.5f, 3, 0.5f) };
        this.wallMesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        this.wallMesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(3, 0), new Vector2(3, 1) };
        // Create the parents
        this.floorParent = new GameObject("Floor");
        this.ceilingParent = new GameObject("Ceiling");
        this.wallsParent = new GameObject("Walls");

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
