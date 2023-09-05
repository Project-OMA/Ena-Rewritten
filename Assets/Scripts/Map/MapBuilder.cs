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
    [SerializeField] private float ceilingHeigth = 3;
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
        floorPiece = new GameObject("Floor:" + startArr[0] + "_" + startArr[1] + "_" + endArr[0] + "_" + endArr[1]);
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
    {
        Vector3 start = new Vector3(startArr[0], 1, -startArr[1]);
        Vector3 end = new Vector3(endArr[0] + 1, 1, -endArr[1] - 1);
        Vector3 center = (end - start) / 2;
        Vector3 size = new Vector3(Mathf.Abs(end.x - start.x), 1, Mathf.Abs(end.z - start.z));
        GameObject wallPiece = null;

        return; //TODO: Remove this line to enable walls

        // get the material
        Material material = null;
        try
        {
            material = wallMaterialData.GetMaterial(code);
        }
        catch (System.Exception)
        {
            Debug.LogError("Material " + code + " not found");
            material = defaultWallMaterial;
        }

        // make 4 copies of the wall mesh and rotate them accordingly
        Mesh meshFront = new Mesh();
        Mesh meshBack = new Mesh();
        Mesh meshLeft = new Mesh();
        Mesh meshRight = new Mesh();

        meshFront.vertices = wallMesh.vertices;
        meshFront.triangles = wallMesh.triangles;
        meshFront.uv = wallMesh.uv;
        
        meshBack.vertices = wallMesh.vertices;
        meshBack.triangles = wallMesh.triangles;
        meshBack.uv = wallMesh.uv;
        
        meshLeft.vertices = wallMesh.vertices;
        meshLeft.triangles = wallMesh.triangles;
        meshLeft.uv = wallMesh.uv;

        meshRight.vertices = wallMesh.vertices;
        meshRight.triangles = wallMesh.triangles;
        meshRight.uv = wallMesh.uv;

        // scale the mesh uvs
        var newUvsFront = new Vector2[meshFront.uv.Length];
        var newUvsBack = new Vector2[meshBack.uv.Length];
        var newUvsLeft = new Vector2[meshLeft.uv.Length];
        var newUvsRight = new Vector2[meshRight.uv.Length];

        // front and back
        for (var i = 0; i < newUvsFront.Length; i++)
        {
            newUvsFront[i] = new Vector2(meshFront.uv[i].x * size.x, meshFront.uv[i].y * size.y);
            newUvsBack[i] = new Vector2(meshBack.uv[i].x * size.x, meshBack.uv[i].y * size.y);
        }

        // left and right
        for (var i = 0; i < newUvsLeft.Length; i++)
        {
            newUvsLeft[i] = new Vector2(meshLeft.uv[i].x * size.z, meshLeft.uv[i].y * size.y);
            newUvsRight[i] = new Vector2(meshRight.uv[i].x * size.z, meshRight.uv[i].y * size.y);
        }

        meshFront.uv = newUvsFront;
        meshBack.uv = newUvsBack;

        meshLeft.uv = newUvsLeft;
        meshRight.uv = newUvsRight;

        // fix the mesh normals
        meshFront.RecalculateNormals();
        meshBack.RecalculateNormals();
        meshLeft.RecalculateNormals();
        meshRight.RecalculateNormals();

        // create the wall tile
        wallPiece = new GameObject("Wall:" + startArr[0] + "_" + startArr[1] + "_" + endArr[0] + "_" + endArr[1]);
        wallPiece.transform.position = start + center;
        wallPiece.transform.rotation = Quaternion.identity;
        wallPiece.transform.localScale = size;
        wallPiece.transform.parent = wallsParent.transform;
        //add mesh renderer and filter
        wallPiece.AddComponent<MeshRenderer>();
        wallPiece.AddComponent<MeshFilter>();
        wallPiece.AddComponent<MeshCollider>();

        // add the meshes to the wall
        wallPiece.GetComponent<MeshRenderer>().material = material;
        wallPiece.GetComponent<MeshFilter>().mesh = meshFront;
        wallPiece.GetComponent<MeshCollider>().sharedMesh = meshFront;
    }

    private void InstanceCeilingTile(string code, int[] startArr, int[] endArr)
    {
        Vector3 start = new Vector3(startArr[0], ceilingHeigth, -startArr[1]);
        Vector3 end = new Vector3(endArr[0] + 1, ceilingHeigth, -endArr[1] - 1);
        Vector3 center = (end - start) / 2;
        Vector3 size = new Vector3(Mathf.Abs(end.x - start.x), 1, Mathf.Abs(end.z - start.z));

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

        GameObject ceilingPiece = new GameObject("Ceiling:" + startArr[0] + "_" + startArr[1] + "_" + endArr[0] + "_" + endArr[1]);
        ceilingPiece.transform.position = start + center;
        // it needs to be rotated 180 degrees since the mesh is upside down
        ceilingPiece.transform.rotation = Quaternion.Euler(180, 0, 0);
        ceilingPiece.transform.localScale = size;
        ceilingPiece.transform.parent = ceilingParent.transform;
        //add mesh renderer and filter
        ceilingPiece.AddComponent<MeshRenderer>();
        ceilingPiece.AddComponent<MeshFilter>();
        ceilingPiece.AddComponent<MeshCollider>();

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

        


        ceilingPiece.GetComponent<MeshRenderer>().material = material;
        ceilingPiece.GetComponent<MeshFilter>().mesh = mesh;
        ceilingPiece.GetComponent<MeshCollider>().sharedMesh = mesh;
    }


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

        // Build the walls
        if (!disableWalls)
        {
            foreach (Wall wall in walls)
            {
                InstanceWallTile(wall.type, wall.start, wall.end);
            }
        }

        // Build the ceilings
        if (!disableCeilings)
        {
            foreach (Floor floor in floors)
            {
                InstanceCeilingTile(floor.type, floor.start, floor.end);
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
