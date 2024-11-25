using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectLogs : MonoBehaviour
{

    private static readonly Dictionary<int, PlayerEvent> PlayerLogs = InteractionController.PlayerDetects;
    private static readonly Dictionary<string, CollisionEvent> CollisionLogs = HandFeedback.Collisions;

    private int nextUpdate=10;

    public static int lastreadPlayerLogs = 0;
    public static int lastreadCollisionLogs = 0; 


    void OnApplicationQuit(){

        SaveCollisionDataToCsv();

    }

    void Update(){
        
    	if(Time.time>=nextUpdate){
    		Debug.Log(Time.time+">="+nextUpdate);
    		
    		nextUpdate=Mathf.FloorToInt(Time.time)+10;

            SaveCollisionDataToCsv();
    		
    	}
    }

    public static void SaveCollisionDataToCsv(){

                string date = MapLoader.startdate.ToString();
                date = date.Replace('/', '-');
                date = date.Replace(':','-');

                lastreadPlayerLogs = CsvWriter.WriteToCsv(PlayerLogs.Values, "/playerlogs" + date + ".csv", lastreadPlayerLogs);
                lastreadCollisionLogs = CsvWriter.WriteToCsv(CollisionLogs.Values, "/collisionlogs" + date + ".csv", lastreadCollisionLogs);
                
    }
}
