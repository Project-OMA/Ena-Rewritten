using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using HtmlAgilityPack;
using TMPro;
using System.Net.Http;
using UnityEngine.InputSystem;
using System.IO;

public class MenuManager : MonoBehaviour
{

    List<string> mapList = new List<string>();

    public Button yourButton;

    public Button tutButton;
    public GameObject noTut;

    [SerializeField] private TMP_Dropdown dropdown;

    public GameObject menu;

    public ChangeScene changeScene;

    private string map;

    public GameObject connectIssue;

    public GameObject exIssue;

    public TextMeshProUGUI textMeshPro;

    public Transform player;

    public TTSManager tTSManager;

    private string m_Path;

    private bool IsMenu = true;

    
   
    string cursaUrl = "https://cursa.eic.cefet-rj.br/ena-map";

    public void getMapList(string url){

        try
        {

        HtmlWeb web = new HtmlWeb();
        HtmlDocument doc = web.Load(url);

        
        

        
        var tableNodes = doc.DocumentNode.SelectNodes("//table");

       

        if (tableNodes != null && tableNodes.Count > 0)
        {
            // Assuming there is only one table on the page, you can adjust this loop accordingly
            foreach (var table in tableNodes)
            {
                // Select all rows within the table
                var rows = table.SelectNodes(".//tr");

                if (rows != null)
                {
                    foreach (var row in rows)
                    {
                        // Select all cells within the row
                        var cells = row.SelectNodes(".//td | .//th");
                        
                        if (cells != null)
                        {
                            foreach (var cell in cells)
                            {
                            

                                if (cell.InnerText.Contains(".json") || cell.InnerText.Contains(".xml")){
                                    mapList.Add(cell.InnerText.Trim());
                                    
                                }
                            }
                            
                        }
                    }
                }
            }
        } 
        }

        catch (Exception ex){
            try{
                
                MapLoader.hasInternet = false;
                Debug.Log(ex.Message);
                connectIssue.SetActive(value: !connectIssue.activeSelf);
                textMeshPro.text = ex.Message;
                exIssue.SetActive(value: !exIssue.activeSelf);

                m_Path = Application.dataPath;
                Debug.Log("dataPath : " + m_Path);

                TextAsset[] mapFiles = Resources.LoadAll<TextAsset>("MapsNoInternet");

                foreach (TextAsset mapFile in mapFiles)
                    {
                        
                        string fileName = mapFile.name;
                        Debug.Log($"Loaded map file: {fileName}");


                        mapList.Add(fileName);
                    }
                

            }catch (Exception exp){
            Debug.Log(exp.Message);
            }

        }
    }

    public string GetDropdownValue(){

        int pickedEntryIndex = dropdown.value;
        string selectedOption = dropdown.options[pickedEntryIndex].text;
        Debug.Log(selectedOption);
        return selectedOption;

    }
    public void TaskOnClick(){

            Debug.Log("AAAAAAA");

            map = GetDropdownValue();
		    
            changeScene.scene_changer("MainScene", map);
        
    }
    
    public void ExitGame(){
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    public void GoToTutorial(){

        if(MapLoader.hasInternet){

            
            TutorialCheckpoints.playerHasMoved = false;
            TutorialCheckpoints.playerHasInteracted = false;
            TutorialCheckpoints.caneActive = false;
            TutorialCheckpoints.playerOnTrigger = false;
            TutorialCheckpoints.playerDoor = false;

            TutorialCheckpoints.playerInTutorial = true;
            changeScene.Tutorial("TutorialScene");

        }else{

            noTut.SetActive(value: !noTut.activeSelf);
            tutButton.enabled = false;
            tutButton.gameObject.SetActive(false);

        }
        

    }
    public void poupulateDropdown(){

        getMapList(cursaUrl);

        if(!mapList.Any()){
            dropdown.options.Add(item: new TMP_Dropdown.OptionData(text: MapLoader.map, image: null));

        }

        foreach(string map in mapList){

            if(map.Contains(".xml")){
                Sprite imageSprite = Resources.Load<Sprite>("Images/" + "xml");
                dropdown.options.Add(item: new TMP_Dropdown.OptionData(text: map, image: imageSprite));
            }else{
                Sprite imageSprite = Resources.Load<Sprite>("Images/" + "json");
                dropdown.options.Add(item: new TMP_Dropdown.OptionData(text: map, image: imageSprite));
            }
            
        }

    }

    private void Awake(){
        poupulateDropdown();
        dropdown.RefreshShownValue(); 
        MapLoader.hasMenu = IsMenu;
        
    }

    private void Start(){

        tTSManager.TTSMenu(true);

        Button btn = yourButton.GetComponent<Button>();
        

        btn.onClick.AddListener(TaskOnClick);
    }

    private void Update(){

        
            menu.transform.position = player.position+ new Vector3(x: player.forward.x, y: 1.0f, z: player.forward.z).normalized*1.5f;
            menu.transform.LookAt(worldPosition: new Vector3(x: player.position.x, y: menu.transform.position.y, z: player.position.z) );
            menu.transform.forward *=-1;
        
    }

    
    

}

