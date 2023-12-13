using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
//using System.Text.Json.Serialization;
using UnityEngine;
using JsonUtility = UnityEngine.JsonUtility;

[Serializable]
public class MapJson
{
    public int id_map;
    public int id_user;
    public int id_owner;
    public string name;
    public string json;
}

public class MapPuller
{
    private string email;
    private int idMap;

    public MapPuller(string email)
    {
        this.email = email;
    }

    public string GetNextMap()
    {
        try
        {
            string apiUrl = "https://achernar.eic.cefet-rj.br/mapserverapi/pub/groups/next-map/";
            using HttpClient httpClient = new HttpClient();
            var response = httpClient.GetAsync(apiUrl + this.email).Result;
            response.EnsureSuccessStatusCode();

            string jsonString = response.Content.ReadAsStringAsync().Result;
            Debug.Log(jsonString);
            var data = JsonUtility.FromJson<MapJson>(jsonString);
            //MapJson jsonFormated = JsonSerializer.Deserialize<MapJson>(jsonString);
            this.idMap = data.id_map;

            var json = data.json;
            Debug.Log(json);
            return json;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ocorreu um erro ao obter o próximo mapa: " + ex.Message);
            throw;
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
