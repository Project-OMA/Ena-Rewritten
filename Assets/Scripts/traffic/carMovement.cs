using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.XR;
using System.Linq;

public class carmovement : MonoBehaviour
{
    // Start is called before the first frame update
    public int speed = 2;

    void Update()
    {

        if(trafficController.canMove && !playerOnTrafficSign.playerCrossing){
          transform.Translate(Vector3.right * speed * Time.deltaTime);  
        }
        
        
    }
}
