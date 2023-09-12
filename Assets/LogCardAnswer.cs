using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static startCSV;
using static colisaocarta;

public class LogCardAnswer : MonoBehaviour
{
    public GameObject cardObject;
    private LogCardData cardData;

    void OnTriggerExit()
    {
        cardData = cardObject.GetComponent<LogCardData>();

        using (StreamWriter sw = File.AppendText("data.csv"))
        {
            sw.WriteLine("Player1,{0},{1},{2}", cardData.cardID, cardData.correctAnswer, gameObject.name);
        };

        Destroy(cardObject);
    }
}
