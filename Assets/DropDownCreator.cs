using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HtmlAgilityPack;
using TMPro;



public class DropdownCreator : MonoBehaviour
{

    List<string> mapList = new List<string>();

    public Button yourButton;

    [SerializeField] private TMP_Dropdown dropdown;

    public ChangeScene changeScene;

    private string map;

   
    string cursaUrl = "https://cursa.eic.cefet-rj.br/ena-map";

    public void getMapList(string url){

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

                                if (cell.InnerText.Contains(".json")){
                                    mapList.Add(cell.InnerText.Trim());
                                }
                            }
                            
                        }
                    }
                }
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
    void Start()
    {
        getMapList(cursaUrl);

        Button btn = yourButton.GetComponent<Button>();
        

		btn.onClick.AddListener(TaskOnClick);

        Debug.Log(dropdown);

        foreach(string map in mapList){
            
            dropdown.options.Add(item: new TMP_Dropdown.OptionData(text: map, image: null));


        }       
    }

}

