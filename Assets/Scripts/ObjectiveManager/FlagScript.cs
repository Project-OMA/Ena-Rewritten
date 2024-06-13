using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlagScript : MonoBehaviour
{
    // Start is called before the first frame update
    
    

    AudioSource m_MyAudioSource;

    private ChangeScene changeScene;

    void Start()
    {
        //Fetch the AudioSource from the GameObject
        m_MyAudioSource = GetComponent<AudioSource>();
        
    }

    public float targetTime = 7.0f ;
    bool finished;


    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }


    void OnTriggerEnter(Collider playCollider){

            if(playCollider.gameObject.tag == "Player"){
                
                m_MyAudioSource.Play();
                finished=true;
                
                
            }  
        }

    // Update is called once per frame
    void Update()
    {
        if(finished){
            targetTime -= Time.deltaTime;

		if (targetTime <= 0.0f) {

            string mapLoad = MapLoader.map;

            if(mapLoad == "default"){
                QuitGame();
            }else{
                SceneManager.LoadScene("MainMenu");
            }
			        
                    
		        }

        }
        

        
        
    }
}
