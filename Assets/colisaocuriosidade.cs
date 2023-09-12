using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;


public class colisaocuriosidade : MonoBehaviour {
    private bool register = true;
    private bool gamb = true;
    /**
    public float timeRemaining = 10;
    public bool timerIsRunning = false;*/
    public GameObject Carta;


    [SerializeField]
    VideoPlayer myVideoPlayer;


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "peao")
        {
            if (gamb)
            {

                Carta.SetActive(false);
                /**timerIsRunning = true;*/
                Debug.Log(Carta);
                Debug.Log("colidiu......");
                gamb = false;
            }
        }
    }

    void OnTriggerExit()
    {
        Debug.Log("saiuu");
        gamb = true;
    }

    void Start()
    {
                  if (register)
            {
                myVideoPlayer.loopPointReached += DoSomethingWhenVideoFinish;
                register = false;
            }
    }


    void DoSomethingWhenVideoFinish(VideoPlayer vp)
    {
        Debug.Log("uhuuuuuuuuuuuuuuuuuuuuullllllllll");
        Carta.SetActive(true);
        Debug.Log(Carta);
    }


    /**
    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;
                
                
            }
        }
    }


    */
      }