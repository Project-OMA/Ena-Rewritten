using System;
using UnityEngine;

public class PlayerEvent
{

    [CsvColumn("CurrentPlayerTime")]
    public DateTime PlayerTime { get; }



    [CsvColumn("CurrentMap")]
    public string CurrentMap { get; }



    [CsvColumn("Player Position")]
    public Vector3 Vector3 { get; set; }


    public PlayerEvent(string currentMap,
                            Vector3 vector3)
    {
    
        Vector3 = vector3;
        PlayerTime = DateTime.Now;
        CurrentMap = currentMap;
    }
}
