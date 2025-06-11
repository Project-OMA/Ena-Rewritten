using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapLoader : MonoBehaviour
{
    public static string mapMenu = "default";

    public static List<string> defaultMapList = new List<string>();

    public static string mapNoMenu = "default";

    public static string beforeMap = "default";

    public static int mapselected = 0;

    public static bool played = false;

    public static bool hasMenu = false;

    public static bool hasInternet = true;

    public static DateTime startdate = DateTime.Now;

    public static bool hasPlayedIntro;

    public static int lineCount = 0;

    public static bool isInMenu = false;

    public static bool playerInTraffic = false;

    public static bool playerInMain = false;

    public static bool HasOneMap = false;

    public static bool basicTest = false;


    

}
