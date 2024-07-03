using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MenuVoice : MonoBehaviour
{
    // Start is called before the first frame update

    public TTSManager tTSManager;
    void Start()
    {
        if(MapLoader.map == "default" && MapLoader.mapdefault !="default"){

            if(!MapLoader.played){
                MapLoader.played = true;
                tTSManager.TTSMenu(false);
                tTSManager.MapLoaded("Map loaded " + MapLoader.mapdefault);
            }else{
                tTSManager.MapLoaded("Map loaded " + MapLoader.mapdefault);
            }
            
        }else if(MapLoader.map != "default" && MapLoader.mapdefault =="default"){

                tTSManager.MapLoaded("Map loaded " + MapLoader.map);
                
        }



        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
