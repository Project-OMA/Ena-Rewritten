using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using JsonUtility = UnityEngine.JsonUtility;
using HtmlAgilityPack;
using System.Linq;
using System.IO;



[Serializable]
public class MapJson
{
    public int id_map;
    public int id_user;
    public int id_owner;
    public string name;
    public string json;
}

enum PullState{
    ERROR,
    RUNNING
}

public class MapPuller
{
    private string email;
    private int idMap;

    private string m_Path;

    private string defaultMapPath;

    private TextAsset defaultMapFile;
    List<string> mapList = new List<string>();

    private PullState pullState;
    
    public MapPuller(string email)
    {
        this.email = email;
    }

    string cursaUrl = "https://cursa.eic.cefet-rj.br/ena-map";

    public void getDefaultMapList(string url){

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

            try{

                m_Path = Application.dataPath;
                Debug.Log("dataPath : " + m_Path);

                string[] files = Directory.GetFiles("Assets/Resources/MapsNoInternet");

                foreach (string file in files){

                    string fileName = Path.GetFileName(file);
                    Debug.Log(fileName);
                
                
                    if (!fileName.Contains(".meta")){
                            
                            mapList.Add(fileName);
                                    
                    }

                }

            }catch (Exception exp){
            Debug.Log(exp.Message);
            }

        }
    }

    

    public string GetNextMap()
    {
        try
        {
            Debug.Log("Obtendo mapa a partir do servidor");
            string apiUrl = "https://cursa.eic.cefet-rj.br/ena-map";
            

            string mapLoad = MapLoader.map;

            if(mapLoad == "default" && MapLoader.mapdefault == "default"){

                getDefaultMapList(cursaUrl);
                Debug.Log("MAP"+mapList);
                MapLoader.defaultMapList = mapList;

                if(MapLoader.defaultMapList.Any()){

                    
                    Debug.Log("DEFAULT:"+MapLoader.defaultMapList);
                    MapLoader.mapdefault = MapLoader.defaultMapList[MapLoader.mapselected];

                    MapLoader.mapselected +=1;
                    mapLoad = MapLoader.mapdefault;

                }else{

                    throw new Exception("Faulty connection");
                }
                
            }else if(MapLoader.mapdefault != "default"){
                mapLoad = MapLoader.mapdefault;
            }

            apiUrl = apiUrl + "/" + mapLoad;
            using HttpClient httpClient = new HttpClient();
            var response = httpClient.GetAsync(apiUrl).Result;
            response.EnsureSuccessStatusCode();

            string jsonString = response.Content.ReadAsStringAsync().Result;
            Debug.Log(jsonString);
            return jsonString;
            
        }
        catch (Exception ex)
        {
            string mapLoad = MapLoader.map;

            if(mapLoad == "default" && MapLoader.mapdefault == "default"){

                Debug.Log("MAP"+mapList);
                MapLoader.defaultMapList = mapList;

                if(MapLoader.defaultMapList.Any()){

                    
                    Debug.Log("DEFAULT:"+MapLoader.defaultMapList);
                    MapLoader.mapdefault = MapLoader.defaultMapList[MapLoader.mapselected];

                    MapLoader.mapselected +=1;
                    mapLoad = MapLoader.mapdefault;

                }else{

                    throw new Exception("No default Maps!");
                }
                
            }else if(MapLoader.mapdefault != "default"){
                mapLoad = MapLoader.mapdefault;
            }

            m_Path = Application.dataPath;
            Debug.Log("dataPath : " + m_Path);
            mapLoad = Path.GetFileNameWithoutExtension(mapLoad);
            Debug.Log("MapLoad: "+ mapLoad);

            defaultMapFile = Resources.Load<TextAsset>("MapsNoInternet/"+mapLoad);
            Debug.Log("MAPFILE:" + defaultMapFile);
            return defaultMapFile.text;
        }
    }

    public async void FinishMap()
    {
        try
        {
            // URL da API que fornece o próximo mapa em JSON (substitua pela URL real)
            string apiUrl = "https://achernar.eic.cefet-rj.br/mapserverapi/pub/groups/";

            using HttpClient httpClient = new HttpClient();

            string url = $"{apiUrl}/{this.idMap}/{this.email}/save";

            HttpResponseMessage response = await httpClient.PutAsync(url, null);

            response.EnsureSuccessStatusCode();
            Console.WriteLine("Mapa Finalizado com sucesso.");
        }
        catch (Exception ex)
        {
            // Lida com erros de solicitação
            Console.WriteLine("Ocorreu um erro para finalizar mapa: " + ex.Message);
            throw;
        }
    }
}
