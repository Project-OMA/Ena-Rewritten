using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour 
{
    public float andar = 1;
   private int velocidadeFrente;
   private int velocidadeTras;
    public GameObject cam;
   private int velocidadeLateral;

   void Start (){
      velocidadeFrente = 10;
      velocidadeTras = 5;
      velocidadeLateral = 2;
   }

   void Update () {
    float x = Input.GetAxis ("Vertical");
    var control = new Vector3(0,x,0);

    Vector3 right = cam.transform.right;
    Vector3 forward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
    Vector3 moveVector = forward * control.y + right * control.x;
    moveVector.y = 0;
    transform.Translate(moveVector * andar * Time.deltaTime, Space.World);
    }
}

