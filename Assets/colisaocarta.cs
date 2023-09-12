using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colisaocarta : MonoBehaviour
{
    public GameObject Desafio;

    public Transform Desafiot;

    void OnTriggerEnter(Collider other)
    {
        Instantiate(Desafio, Desafiot.position, Desafiot.rotation);

    }

}
