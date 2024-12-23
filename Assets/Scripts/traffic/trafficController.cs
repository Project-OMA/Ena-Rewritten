using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.XR;
using System.Linq;

public class trafficController : MonoBehaviour
{
    public static bool canMove = true;

    private int nextUpdate=20;

    private int restartDelay=10;

    public GameObject trigger;

    private AudioClip clip;

    void Start(){
        trigger.SetActive(false);

        
    
    }

    void Update(){

        if(Time.time>=nextUpdate){
    		Debug.Log(Time.time+">="+nextUpdate);
    		
    		nextUpdate=Mathf.FloorToInt(Time.time)+20;

            trigger.SetActive(true);
            Invoke(nameof(RestartCar), restartDelay);

    		
    	}
    }

    private void RestartCar()
    {
        canMove = true; 
        trigger.SetActive(false);
    }
}
