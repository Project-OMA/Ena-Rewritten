using System;
using UnityEngine;

public class VibrationEvent
{

    [CsvColumn("CurrentPlayerTime")]
    public DateTime PlayerTime { get; }

    [CsvColumn("Time To Win")]
    public float TimeToWin { get; set; }


    [CsvColumn("Time Vibrating")]
    public float TimeVibrating { get; set; }

    [CsvColumn("Status")]
    public string Status {get; set; }

    [CsvColumn("BoxSize")]
    public Vector3 BoxSize {get;}


    public VibrationEvent(float timeVibrating,
                        float timeToWin,
                        string status
    )
    {
        PlayerTime = DateTime.Now;
        TimeToWin = timeToWin;
        TimeVibrating = timeVibrating;
        Status = status;
        BoxSize = ColliderResizer.boxSize;

    }
}
