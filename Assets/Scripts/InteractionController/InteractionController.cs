using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    public float correr = 2, andar = 1;
   private int velocidadeFrente;
   private int velocidadeTras;

   private int velocidadeLateral;

   void Start (){
      velocidadeFrente = 10;
      velocidadeTras = 5;
      velocidadeLateral = 2;
   }

   void Update () {
    if(Input.GetKey("w")){
         
        transform.Translate(0,0,andar*velocidadeFrente*Time.deltaTime);
            
         
    }
    if(Input.GetKey("s")){
         
        transform.Translate(0,0,-andar*velocidadeTras*Time.deltaTime);
      
    }

    if(Input.GetKey("d")){
         
        transform.Rotate(0,(-andar*velocidadeLateral*Time.deltaTime),0);
            
         
    }
    if(Input.GetKey("a")){
         
        transform.Rotate(0,(andar*velocidadeLateral*Time.deltaTime),0);
      
    }

      
   }
}
