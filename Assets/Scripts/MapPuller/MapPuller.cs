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

            return response.Content.ReadAsStringAsync().Result;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ocorreu um erro ao obter o próximo mapa: " + ex.Message);
            throw;
        }
    }
}
