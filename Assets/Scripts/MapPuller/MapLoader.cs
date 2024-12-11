using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapLoader : MonoBehaviour
{
    public static string map = "default";

    public static List<string> defaultMapList = new List<string>();

    public static string mapdefault = "default";

    public static int mapselected = 0;

    public static bool played = false;

    public static bool hasMenu = false;

    public static bool hasInternet = true;

    public static DateTime startdate = DateTime.Now;

    public static bool hasPlayedIntro;

    public static int lineCount=0; 

    public static bool isInMenu = false; 


    

}
