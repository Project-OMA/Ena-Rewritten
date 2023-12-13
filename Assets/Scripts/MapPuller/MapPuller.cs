using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using UnityEngine;


public class MapJson
{
    public int id_map { get; set; }
    public int id_user { get; set; }
    public int id_owner { get; set; }
    public string name { get; set; }
    public string json { get; set; }
}

public class MapPuller : MonoBehaviour
{
    private readonly string Email;
    private readonly int idMap;

    public MapPuller(string email)
    {
        this.Email = email;
    }

    void Start()
    {
        this.GetNextMap();
    }

    public string GetNextMap()
    {
        try
        {
            string apiUrl = "https://achernar.eic.cefet-rj.br/mapserverapi/pub/groups/next-map/";
            using HttpClient httpClient = new HttpClient();
            var response = httpClient.GetAsync(apiUrl + this.Email).Result;
            response.EnsureSuccessStatusCode();

            string jsonString =  response.Content.ReadAsStringAsync().Result;
            MapJson jsonFormated = JsonSerializer.Deserialize<MapJson>(jsonString);
            this.idMap = jsonFormated.id_map;

            return jsonFormated.json;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ocorreu um erro ao obter o próximo mapa: " + ex.Message);
            throw;
        }
    }

    public async void FinishMap(){
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
        catch (Exception ex){
            // Lida com erros de solicitação
            Console.WriteLine("Ocorreu um erro para finalizar mapa: " + ex.Message);
            throw;
        }
  }
}
