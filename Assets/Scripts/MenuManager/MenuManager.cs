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


public class MenuManager : MonoBehaviour
{

    List<string> mapList = new List<string>();

    public Button yourButton;

    [SerializeField] private TMP_Dropdown dropdown;

    public GameObject menu;

    public ChangeScene changeScene;

    private string map;

    public GameObject connectIssue;

    public GameObject exIssue;

    public TextMeshProUGUI textMeshPro;

    public Transform player;

    public InputActionProperty showButton;

    

   
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
                                // Output the text content of each cell
                                Debug.Log(cell.InnerText.Trim() + "\t");

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
            Debug.Log(ex.Message);
            connectIssue.SetActive(value: !connectIssue.activeSelf);
            textMeshPro.text = ex.Message;
            exIssue.SetActive(value: !exIssue.activeSelf);
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
    public void poupulateDropdown(){

        getMapList(cursaUrl);

        if(!mapList.Any()){
            dropdown.options.Add(item: new TMP_Dropdown.OptionData(text: MapLoader.map, image: null));
        }

        foreach(string map in mapList){

            if(map.Contains(".json")){
                Sprite imageSprite = Resources.Load<Sprite>("Images/" + "json");
                dropdown.options.Add(item: new TMP_Dropdown.OptionData(text: map, image: imageSprite));
            }else{
                Sprite imageSprite = Resources.Load<Sprite>("Images/" + "xml");
                dropdown.options.Add(item: new TMP_Dropdown.OptionData(text: map, image: imageSprite));
            }
            
        }

    }

    private void Awake(){
        poupulateDropdown();
        dropdown.RefreshShownValue(); 
        
    }

    private void Start(){
        Button btn = yourButton.GetComponent<Button>();
        

        btn.onClick.AddListener(TaskOnClick);
    }

    private void Update(){

        if(showButton.action.WasPressedThisFrame()){
            menu.transform.position = player.position+ new Vector3(x: player.forward.x, y: 0, z: player.forward.z).normalized*2;
            menu.transform.LookAt(worldPosition: new Vector3(x: player.position.x, y: menu.transform.position.y, z: player.position.z) );
            menu.transform.forward *=-1;
        }
    }

    
    

}

