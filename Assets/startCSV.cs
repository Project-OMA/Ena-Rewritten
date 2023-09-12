using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class startCSV : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        List<string[]> data = new List<string[]>();
        data.Add(new string[] { "Player", "Questao", "RespostaCorreta", "RespostaPlayer" });
        using (StreamWriter writer = new StreamWriter("data.csv"))
        {
            foreach (string[] row in data)
            {
                writer.WriteLine(string.Join(",", row));
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
