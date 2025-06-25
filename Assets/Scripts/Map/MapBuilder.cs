using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using MapObjects;
using MapParser;
using System.Linq;
using System.Text.RegularExpressions;

public class MapBuilder : MonoBehaviour
{

    [SerializeField] bool disableWalls = false;
    [SerializeField] bool disableFloors = false;
    [SerializeField] bool disableCeilings = false;
    [SerializeField] bool disableDoorWindow = false;
    [SerializeField] bool disableFurniture = false;
    [SerializeField] bool disableUtensil = false;
    [SerializeField] bool disableElectronic = false;
    [SerializeField] bool disableGoal = false;

    [FormerlySerializedAs("jsonRawFile")]
    [SerializeField] private TextAsset mapFile;

    [SerializeField] private MaterialData floorMaterialData;
    [SerializeField] private MaterialData wallMaterialData;
    [SerializeField] private MaterialData ceilingMaterialData;

    [SerializeField] private ObjectData doorWindowObjectData;
    [SerializeField] private ObjectData furnitureObjectData;
    [SerializeField] private ObjectData utensilObjectData;
    [SerializeField] private ObjectData electronicObjectData;
    [SerializeField] private ObjectData goalObjectData;
    [SerializeField] private PlayerData playerObjectData;
    [SerializeField] private Material defaultFloorMaterial;
    [SerializeField] private Material defaultWallMaterial;
    [SerializeField] private Material defaultCeilingMaterial;
    [SerializeField] private float ceilingHeight = 3.0f;
    [SerializeField] private float floorTextureScale = .75f;
    [SerializeField] private float wallTextureScale = 2.0f;
    [SerializeField] private float ceilingTextureScale = 1f;

    [SerializeField] private bool useGrass = true;
    [SerializeField] private string grassID = "3.1";

    [SerializeField] GameObject grassSprite;

    public bool isTutorial;
    public bool isTraffic;
    public bool mainScene;

    public bool basicTest;
    private Mesh floorMesh;
    private Mesh wallMesh;
    private Mesh objMesh;

    private GameObject floorParent;
    private GameObject ceilingParent;
    private GameObject wallsParent;
    private GameObject doorWindowParent;
    private GameObject furnitureParent;
    private GameObject utensilParent;
    private GameObject electronicParent;
    private GameObject goalParent;
    private GameObject playerParent;
    private string mapData;
    private string defaultMapPath;
    private TextAsset defaultMapFile;

    private void InstanceFloorTile(Floor floor)
    {
        string code = floor.type;
        int[] startArr = floor.start;
        int[] endArr = floor.end;

        Vector3 start = new Vector3(startArr[0], 0, -startArr[1]);
        Vector3 end = new Vector3(endArr[0] + 1, 0, -endArr[1] - 1);
        Vector3 center = (end - start) / 2;
        Vector3 size = new Vector3(Mathf.Abs(end.x - start.x), 1, Mathf.Abs(end.z - start.z));


        // get the material data
        Material material = null;
        bool useGlobalUV = false;
        Vector2 scale = Vector2.one;
        FeedbackSettings feedbackSettings = new FeedbackSettings();
        try
        {
            material = floorMaterialData.GetMaterial(code);
            useGlobalUV = floorMaterialData.DoesMaterialUseGlobalUV(code);
            scale = floorMaterialData.GetMaterialScale(code);
            feedbackSettings = floorMaterialData.GetFeedbackSettings(code);
        }
        catch (System.Exception)
        {
            Debug.LogError("Floor material " + code + " not found");
            material = defaultFloorMaterial;
        }

        // make a copy of the floor mesh
        Mesh mesh = new Mesh
        {
            vertices = floorMesh.vertices,
            triangles = floorMesh.triangles,
            uv = floorMesh.uv
        };
        // scale the mesh uvs
        var newUvs = new Vector2[mesh.uv.Length];
        for (var i = 0; i < newUvs.Length; i++)
        {
            newUvs[i] = new Vector2(mesh.uv[i].x * size.x, mesh.uv[i].y * size.z);
        }

        // apply the scale
        for (var i = 0; i < newUvs.Length; i++)
        {
            newUvs[i] *= scale;
        }

        // if the material uses global uv, move the uvs to the of the "world"
        if (useGlobalUV)
        {
            for (var i = 0; i < newUvs.Length; i++)
            {
                newUvs[i] += new Vector2(start.x, start.z);
            }
        }


        mesh.uv = newUvs;
        // fix the mesh normals
        mesh.RecalculateNormals();

        string materialname;
        materialname = RemoveUnityEngineMaterial(material.ToString());

        // create the floor tile
        var floorPiece = new GameObject("Floor "+materialname+":" + startArr[0] + "_" + startArr[1] + "_" + endArr[0] + "_" + endArr[1]);
        floorPiece.transform.position = start + center;
        floorPiece.transform.rotation = Quaternion.identity;
        floorPiece.transform.localScale = size;
        floorPiece.transform.parent = floorParent.transform;
        //add mesh renderer and filter
        floorPiece.tag = "floor";
        AddComponentsToMaterial(floorPiece);

        MaterialConfigComponents(material, feedbackSettings, mesh, floorPiece);

        if (code == grassID && useGrass)
        {
            float density = size.x * size.z;
            for (int i = 0; i < density; i++)
            {
                float x = Random.Range(start.x, end.x);
                float z = Random.Range(start.z, end.z);
                float y = 0.01f;
                Vector3 position = new Vector3(x, y, z);
                // create cube
                GameObject grass = Instantiate(grassSprite, position, Quaternion.identity);
                grass.transform.parent = floorPiece.transform;
            }

        }
    }

    private static void MaterialConfigComponents(Material material, FeedbackSettings feedbackSettings, Mesh mesh, GameObject obj)
    {
        obj.GetComponent<MeshRenderer>().material = material;
        obj.GetComponent<ObjectFeedbackSettings>().settings = feedbackSettings;
        obj.GetComponent<MeshFilter>().mesh = mesh;

       
        
        Vector3 newCenter = new Vector3(0, -0.025f, 0);
        Vector3 newSize = new Vector3(1, 0.05f, 1);
        BoxCollider floorBox = obj.GetComponent<BoxCollider>();
        floorBox.center = newCenter;
        floorBox.size = newSize;


        

        
        
    }
    
    private static void MaterialConfigComponentsWall(Material material, FeedbackSettings feedbackSettings, Mesh mesh, GameObject obj, string wallPos)
    {
        obj.GetComponent<MeshRenderer>().material = material;
        obj.GetComponent<ObjectFeedbackSettings>().settings = feedbackSettings;
        obj.GetComponent<MeshFilter>().mesh = mesh;




        if ((wallPos == "vertical" || wallPos == "horizontal") && !(obj.name.Contains("Front") || (obj.name.Contains("Back"))))
        {
            Vector3 newCenter = new Vector3(-0.45f, 0.5f, 0);
            Vector3 newSize = new Vector3(0.1f, 1, 1);
            BoxCollider WallBox = obj.GetComponent<BoxCollider>();
            WallBox.center = newCenter;
            WallBox.size = newSize;


        }

        if (obj.name.Contains("Front") || (obj.name.Contains("Back")))
        {
            Vector3 newCenter = new Vector3(-0.5f, 0.5f, 0);
            Vector3 newSize = new Vector3(0.02f, 1, 1);
            BoxCollider WallBox = obj.GetComponent<BoxCollider>();
            WallBox.center = newCenter;
            WallBox.size = newSize;


        }


            

       

        
        
    }


    



    private void InstanceWallTile(Wall wall)
    {
        string wallPos = "";
        string code = wall.type;
        int[] startArr = wall.start;
        int[] endArr = wall.end;

        Vector3 start = new Vector3(startArr[0], 0, -startArr[1]);
        Vector3 end = new Vector3(endArr[0] + 1, 0, -endArr[1] - 1);
        Vector3 center = (end - start) / 2;
        var size = new Vector3(
            Mathf.Abs(end.x - start.x),
            ceilingHeight,
            Mathf.Abs(end.z - start.z)
        );

        bool isHorizontal = wall.start[1] == wall.end[1]; // Same row (Z), changes in X
        bool isVertical = wall.start[0] == wall.end[0];   // Same column (X), changes in Z

        if (isHorizontal)
        {
            wallPos = "horizontal";
        }

        if (isVertical)
        {
            wallPos = "vertical";
        }

        // get the material data
        Material material = null;
        FeedbackSettings feedbackSettings = null;
        //bool useGlobalUV = false;
        //Vector2 scale = Vector2.one;
        try
        {
            material = wallMaterialData.GetMaterial(code);
            feedbackSettings = wallMaterialData.GetFeedbackSettings(code);
            //useGlobalUV = floorMaterialData.DoesMaterialUseGlobalUV(code);
            //scale = floorMaterialData.GetMaterialScale(code);
        }
        catch (System.Exception)
        {
            Debug.LogError("Wall material " + code + " not found");
            material = defaultWallMaterial;
        }

        // make 4 copies of the wall mesh and rotate them accordingly

        var meshFront = new Mesh
        {
            vertices = wallMesh.vertices,
            triangles = wallMesh.triangles,
            uv = wallMesh.uv
        };

        var meshBack = new Mesh
        {
            vertices = wallMesh.vertices,
            triangles = wallMesh.triangles,
            uv = wallMesh.uv
        };

        var meshLeft = new Mesh
        {
            vertices = wallMesh.vertices,
            triangles = wallMesh.triangles,
            uv = wallMesh.uv
        };

        var meshRight = new Mesh
        {
            vertices = wallMesh.vertices,
            triangles = wallMesh.triangles,
            uv = wallMesh.uv
        };

        // scale the mesh uvs
        var newUvsFront = new Vector2[meshFront.uv.Length];
        var newUvsBack = new Vector2[meshBack.uv.Length];
        var newUvsLeft = new Vector2[meshLeft.uv.Length];
        var newUvsRight = new Vector2[meshRight.uv.Length];

        // front and back
        for (var i = 0; i < newUvsFront.Length; i++)
        {
            newUvsFront[i] = new Vector2(meshFront.uv[i].x * size.z, meshFront.uv[i].y * size.y);
            newUvsBack[i] = new Vector2(meshBack.uv[i].x * size.z, meshBack.uv[i].y * size.y);

            newUvsFront[i] *= wallTextureScale;
            newUvsBack[i] *= wallTextureScale;
        }
        // left and right
        for (var i = 0; i < newUvsLeft.Length; i++)
        {
            newUvsLeft[i] = new Vector2(meshLeft.uv[i].x * size.x, meshLeft.uv[i].y * size.y);
            newUvsRight[i] = new Vector2(meshRight.uv[i].x * size.x, meshRight.uv[i].y * size.y);

            newUvsLeft[i] *= wallTextureScale;
            newUvsRight[i] *= wallTextureScale;
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

        // create the object and tiles
        var wallObj = new GameObject("Wall:" + startArr[0] + "_" + startArr[1] + "_" + endArr[0] + "_" + endArr[1] + "_" + wallPos)
        {
            tag = "wall"
        };


        string materialname;
        materialname = RemoveUnityEngineMaterial(material.ToString());

        var wallFront = new GameObject("Front " + materialname + ":" + startArr[0] + "_" + startArr[1] + "_" + endArr[0] + "_" + endArr[1]);
        var wallBack = new GameObject("Back " + materialname + ":" + startArr[0] + "_" + startArr[1] + "_" + endArr[0] + "_" + endArr[1]);
        var wallLeft = new GameObject("Left " + materialname + ":" + startArr[0] + "_" + startArr[1] + "_" + endArr[0] + "_" + endArr[1]);
        var wallRight = new GameObject("Right " + materialname + ":" + startArr[0] + "_" + startArr[1] + "_" + endArr[0] + "_" + endArr[1]);
        var wallPieces = new[] { (wallFront, meshFront), (wallBack, meshBack), (wallLeft, meshLeft), (wallRight, meshRight) };


        foreach (var (obj, _) in wallPieces)
        {
            //wallPiece.transform.position = start + center;
            obj.transform.rotation = Quaternion.identity;
            //wallPiece.transform.localScale = size;
            obj.transform.parent = wallObj.transform;
            //add mesh renderer and filter
            obj.tag = "wall";
            AddComponentsToWall(obj, wallPos);

        }

        foreach (var (obj, mesh) in wallPieces)
        {
            MaterialConfigComponentsWall(material, feedbackSettings, mesh, obj, wallPos);
        }

        // rotate the wall pieces
        wallFront.transform.Rotate(0, 0, 0);
        wallBack.transform.Rotate(0, 180, 0);
        wallLeft.transform.Rotate(0, 90, 0);
        wallRight.transform.Rotate(0, -90, 0);


        wallObj.transform.position = start + center;
        wallObj.transform.rotation = Quaternion.identity;
        wallObj.transform.localScale = size;
        wallObj.transform.parent = wallsParent.transform;
    }

    private static void AddComponentsToMaterial(GameObject obj)
    {
        obj.AddComponent<MeshRenderer>();
        obj.AddComponent<ObjectFeedbackSettings>();
        obj.AddComponent<MeshFilter>();

        
        obj.AddComponent<BoxCollider>();

        
    }

    private static void AddComponentsToWall(GameObject obj, string wallPos)
    {

        obj.AddComponent<MeshRenderer>();
        obj.AddComponent<ObjectFeedbackSettings>();
        obj.AddComponent<MeshFilter>();

        
        obj.AddComponent<BoxCollider>();

       
        

        

    }

    private void InstanceCeilingTile(Ceiling ceiling)
    {
        string code = ceiling.type;
        int[] startArr = ceiling.start;
        int[] endArr = ceiling.end;

        Vector3 start = new Vector3(startArr[0], ceilingHeight, -startArr[1]);
        Vector3 end = new Vector3(endArr[0] + 1, ceilingHeight, -endArr[1] - 1);
        Vector3 center = (end - start) / 2;
        Vector3 size = new Vector3(Mathf.Abs(end.x - start.x), 1, Mathf.Abs(end.z - start.z));

        // get the material
        Material material = null;
        FeedbackSettings feedbackSettings = null;
        // bool useGlobalUV = false;
        // Vector2 scale = Vector2.one;
        try
        {
            material = ceilingMaterialData.GetMaterial("0.0");
            feedbackSettings = ceilingMaterialData.GetFeedbackSettings(code);
            // useGlobalUV = floorMaterialData.DoesMaterialUseGlobalUV(code);
            // scale = floorMaterialData.GetMaterialScale(code);
        }
        catch (System.Exception)
        {
            Debug.LogError("Ceiling material " + code + " not found");
            material = defaultCeilingMaterial;
        }

        GameObject ceilingPiece = new GameObject("Ceiling:" + startArr[0] + "_" + startArr[1] + "_" + endArr[0] + "_" + endArr[1]);
        ceilingPiece.transform.position = start + center;
        ceilingPiece.tag = "ceiling";
        // it needs to be rotated 180 degrees since the mesh is upside down
        ceilingPiece.transform.rotation = Quaternion.Euler(180, 0, 0);
        ceilingPiece.transform.localScale = size;
        ceilingPiece.transform.parent = ceilingParent.transform;
        //add mesh renderer and filter

        AddComponentsToMaterial(ceilingPiece);

        // make a copy of the floor mesh
        Mesh mesh = new Mesh
        {
            vertices = floorMesh.vertices,
            triangles = floorMesh.triangles,
            uv = floorMesh.uv
        };
        // scale the mesh uvs
        var newUvs = new Vector2[mesh.uv.Length];
        for (var i = 0; i < newUvs.Length; i++)
        {
            newUvs[i] = new Vector2(mesh.uv[i].x * size.x, mesh.uv[i].y * size.z);
            newUvs[i] *= ceilingTextureScale;
        }

        mesh.uv = newUvs;


        // fix the mesh normals
        mesh.RecalculateNormals();

        MaterialConfigComponents(material, feedbackSettings, mesh, ceilingPiece);

    }

    private void InstanceProp(MapProp prop, ObjectData objectData, GameObject parent = null, string tag = "")
    {
        string type = prop.getType();
        int[] pos = prop.getPos();
        ObjectPrefab propData = null;
        GameObject prefab = null;
        FeedbackSettings feedbackSettings = new FeedbackSettings();
        try
        {
            propData = objectData.GetObject(type);
            prefab = propData.prefab;
            
        }
        catch (System.Exception)
        {
            Debug.LogError("Prefab not found for type " + type + " in data file " + objectData);
            return;
        }

        feedbackSettings = objectData.GetFeedbackSettings(propData.name);



        string name = prefab.name + ":" + type + "_" + pos[0] + "_" + pos[1];

        // Position
        float posX = pos[0] + propData.offsetX;
        float posY = -pos[1] + propData.offsetY;
        Vector3 vecpos = new Vector3(posX, 0, posY);

        // Rotation
        Quaternion rot = Quaternion.Euler(propData.rotationx, propData.rotation, propData.rotationz);

        // Create the object
        GameObject obj = Instantiate(prefab, vecpos, rot);
        // Debug.LogWarning($"nome: {.name} quantidade: {obj.GetComponents<MeshCollider>().Count()}");
        // Debug.LogWarning($"nome: {obj.name} quantidade: {prefab.GetComponents<MeshCollider>().Count()}");

        obj.name = name;

        obj.transform.parent = parent.transform;

        if(prefab.tag != "Final"){
          obj.tag = tag;  
        }

        
        obj.AddComponent<ObjectFeedbackSettings>();
        obj.GetComponent<ObjectFeedbackSettings>().settings = feedbackSettings;


        if(obj.tag != "Final"){
           AddTagsToChildren(obj.transform, obj.tag); 
        }
        


        // Use this code to remove extra colliders in the prefab
        
        // var meshColliders = prefab.GetComponents<MeshCollider>();
        // if (meshColliders.Count() > 0)
        // {
        //     //Debug.Log("Deleting " + meshColliders.Count() + " colliders");
        //     for (int i = 0; i < meshColliders.Count(); i++)
        //     {
        //         DestroyImmediate(meshColliders[i], true);
        //     }
        // }
        
    }

    public void instancePlayer(MapProp prop, PlayerData playerData, GameObject parent = null, string tag = ""){

        string type = prop.getType();
        int[] pos = prop.getPos();
        ObjectPrefab playerpropData = null;
        GameObject prefab = null;
        

        try
        {
            playerpropData = playerData.GetPlayer(type);
            prefab = playerpropData.prefab;
            
        }
        catch (System.Exception)
        {
            Debug.LogError("Prefab not found for type " + type + " in data file " + playerData);
            return;
        }




        // Position
        float posX = pos[0] + playerpropData.offsetX;
        float posY = -pos[1] + playerpropData.offsetY;
        Vector3 vecpos = new Vector3(posX, 0, posY);

        // Rotation
        Quaternion rot = Quaternion.Euler(playerpropData.rotationx, playerpropData.rotation, playerpropData.rotationz);

        // Create the object
        GameObject player = Instantiate(prefab, vecpos, rot);
        // Debug.LogWarning($"nome: {.name} quantidade: {player.GetComponents<MeshCollider>().Count()}");
        // Debug.LogWarning($"nome: {player.name} quantidade: {prefab.GetComponents<MeshCollider>().Count()}");

        player.transform.parent = parent.transform;
        
        
    }

    public void AddTagsToChildren(Transform parentTransform, string tag = "")
    {
        
        foreach (Transform child in parentTransform)
        {
           
            child.gameObject.tag = tag;
            AddTagsToChildren(child,tag);
        }
    }

    void BuildMap(Map map)
    {
        List<Wall> walls = map.layers.walls;
        List<Floor> floors = map.layers.floors;
        List<Ceiling> ceilings = map.layers.ceilings;
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
                InstanceFloorTile(floor);
            }
        }

        // Build the walls
        if (!disableWalls)
        {
            foreach (Wall wall in walls)
            {
                InstanceWallTile(wall);
            }
        }

        // Build the ceilings
        if (!disableCeilings)
        {
            foreach (Ceiling ceiling in ceilings)
            {
                InstanceCeilingTile(ceiling);
            }
        }

        // Build the doors and windows
        if (!disableDoorWindow)
        {
            foreach (DoorAndWindow obj in door_and_windows)
            {
                InstanceProp(obj, doorWindowObjectData, doorWindowParent, "DoorWindow");

            }
        }

        // Build the furniture
        if (!disableFurniture)
        {
            foreach (Furniture obj in furniture)
            {
                InstanceProp(obj, furnitureObjectData, furnitureParent, "Furniture");
            }
        }

        // Build the utensils
        if (!disableUtensil)
        {
            foreach (Utensil obj in utensils)
            {
                InstanceProp(obj, utensilObjectData, utensilParent, "Utensils");
            }
        }

        // Build the electronics
        if (!disableElectronic)
        {
            foreach (Electronic obj in eletronics)
            {
                InstanceProp(obj, electronicObjectData, electronicParent, "Electronics");
            }
        }

        // Build the goals
        if (!disableGoal)
        {
            foreach (Goal obj in goals)
            {
                InstanceProp(obj, goalObjectData, goalParent, "Goals");
            }
        }

        
        instancePlayer(persons[0], playerObjectData, playerParent, "persons");

        // Set Map object as parent of all layers
        Transform mapTransform = GetComponent<Transform>();
        floorParent.transform.parent = mapTransform;
        ceilingParent.transform.parent = mapTransform;
        wallsParent.transform.parent = mapTransform;
        doorWindowParent.transform.parent = mapTransform;
        furnitureParent.transform.parent = mapTransform;
        utensilParent.transform.parent = mapTransform;
        electronicParent.transform.parent = mapTransform;
        goalParent.transform.parent = mapTransform;
    }

    bool isJson(string data)
    {
        string stripped = data.Trim();
        return stripped[0] == '{';
    }

    bool IsXML(string data)
    {
        string stripped = data.Trim();
        return stripped[0] == '<';
    }


    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        // Floor mesh
        floorMesh = new Mesh
        {
            vertices = new Vector3[] { new Vector3(-0.5f, 0, -0.5f), new Vector3(-0.5f, 0, 0.5f), new Vector3(0.5f, 0, 0.5f), new Vector3(0.5f, 0, -0.5f) },
            triangles = new int[] { 0, 1, 2, 0, 2, 3 },
            uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) }
        };
        // Wall mesh, its the same as the but vertical
        wallMesh = new Mesh
        {
            vertices = new Vector3[] { new Vector3(-0.5f, 0, -0.5f), new Vector3(-0.5f, 0, 0.5f), new Vector3(-0.5f, 1, -0.5f), new Vector3(-0.5f, 1, 0.5f) },
            triangles = new int[] { 0, 1, 2, 3, 2, 1 },
            //this.wallMesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) };

            uv = new Vector2[] { new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 0), new Vector2(1, 0) }
        };


        // Create the parents
        floorParent = new GameObject("Floor");
        ceilingParent = new GameObject("Ceiling");
        wallsParent = new GameObject("Walls");
        doorWindowParent = new GameObject("DoorWindow");
        furnitureParent = new GameObject("Furniture");
        utensilParent = new GameObject("Utensils");
        electronicParent = new GameObject("Electronics");
        goalParent = new GameObject("Goals");
        playerParent = new GameObject("Player");

        MapPuller mapPuller = new MapPuller("joaoaluno@gmail.com");
    

        if(isTutorial){

            MapLoader.playerInMain = false;

            defaultMapPath = "Maps/tutorial";

            Debug.Log("defaultMapPath : " + defaultMapPath);
            defaultMapFile = Resources.Load<TextAsset>(defaultMapPath);
            Debug.Log(defaultMapFile);

            mapData = defaultMapFile.text;

            

            if(MapLoader.hasMenu){

                MapLoader.mapMenu = "tutorial";

            }else{

                if(MapLoader.mapselected!=0){
                    MapLoader.beforeMap = MapLoader.defaultMapList[MapLoader.mapselected];
                }

                MapLoader.mapNoMenu = "tutorial";

            }

        }
        if(isTraffic){

            MapLoader.playerInMain = false;
            Physics.IgnoreLayerCollision(0, 0, false);

            defaultMapPath = "Maps/traffic";

            Debug.Log("defaultMapPath : " + defaultMapPath);
            defaultMapFile = Resources.Load<TextAsset>(defaultMapPath);
            Debug.Log(defaultMapFile);
            

            if(MapLoader.hasMenu){

                MapLoader.mapMenu = "traffictest";

            }else{

                if(MapLoader.mapselected!=0){
                    MapLoader.beforeMap = MapLoader.defaultMapList[MapLoader.mapselected];
                }
                MapLoader.mapNoMenu = "traffictest";

            }

            mapData = defaultMapFile.text;


        }
        if(mainScene){

            MapLoader.playerInMain = true;
            
            Physics.IgnoreLayerCollision(0, 0, false);

            mapData = mapPuller.GetNextMap();

        }

        if (basicTest)
        {

            MapLoader.basicTest = true;
        }





        if (mapFile == null)
        {
            Debug.Log("No map file found, using server/local files");

        }
        else
        {
            MapLoader.HasOneMap = true;
            mapData = mapFile.text;

        }

        IMapParser mapParser = null;



        // Check if the file is JSON or XML
        if (isJson(mapData))
        //if (isJson(mapFile.text))
        {
            Debug.Log("JSON file detected");
            mapParser = new MapParserJSON(mapData);
            //mapParser = new MapParserJSON(mapFile.text);
        }
        else if (IsXML(mapData))
        //else if (IsXML(mapFile.text))
        {
            Debug.Log("XML file detected");
            mapParser = new MapParserXML(mapData);
            //mapParser = new MapParserXML(mapFile.text);
        }
        else
        {
            Debug.LogError("Invalid file format");
            return;
        }
        Map map = mapParser.ParseMap();
        // Build the map
        BuildMap(map);

        
    

    }
    #region utils
    public static string RemoveUnityEngineMaterial(string input)
    {
        // Define the pattern to match "(UnityEngine.Material)"
        string pattern = @"\s*\(UnityEngine\.Material\)";

        // Use Regex to replace the pattern with an empty string
        return Regex.Replace(input, pattern, string.Empty);
    }
    #endregion
}
