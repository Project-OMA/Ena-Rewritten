using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public class MapPuller : MonoBehaviour
{
    private readonly string Email;

    public MapPuller(string email)
    {
        this.Email = email;
    }

    public async Task<string> GetNextMap(){
      try {
        string apiUrl = "https://achernar.eic.cefet-rj.br/mapserverapi/pub/groups/next-map/";

            using HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(apiUrl + this.Email);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            return json;
        } catch (Exception ex){
        Console.WriteLine("Ocorreu um erro ao obter o próximo mapa: " + ex.Message);
        throw;
    }
  }
}
