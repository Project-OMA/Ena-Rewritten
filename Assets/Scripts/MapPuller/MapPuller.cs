using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPuller : MonoBehaviour
{
    private string email;

    public MapPuller(string email)
    {
        this.email = email;
    }


    // Start is called before the first frame update
    void Start()
    {
      this.getNextMap()

    }

    // Update is called once per frame
    void Update()
    {

    }

    public async Task<string> getNextMap(){
      try {
        string apiUrl = "https://achernar.eic.cefet-rj.br/mapserverapi/pub/groups/next-map/";

        using (HttpClient httpClient = new HttpClient()){
          HttpResponseMessage response = await httpClient.GetAsync(apiUrl + this.email);
          response.EnsureSuccessStatusCode();

          string json = await response.Content.ReadAsStringAsync();
          return json;
        }
      } catch (Exception ex){
        Console.WriteLine("Ocorreu um erro ao obter o próximo mapa: " + ex.Message);
        throw;
    }
  }
}
