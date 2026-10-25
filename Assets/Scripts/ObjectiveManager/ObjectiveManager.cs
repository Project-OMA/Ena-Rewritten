using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    private List<GameObject> goals;
    private GameObject player;
    private GameObject nextGoal;
    [SerializeField] private TMP_Text goalText;
    private bool completed = false;


    void Start()
    {
        MapBuilder mapBuilder = GetComponent<MapBuilder>();
        goals = new List<GameObject>(mapBuilder.SpawnedGoals);
        player = mapBuilder.playerReference;
        foreach (TMP_Text text in player.GetComponentsInChildren<TMP_Text>(true))
        {
            if (text.gameObject.name == "TextGoals")
            {
                goalText = text;
                break;
            }
        }
        FindClosestGoal();


    }

    // Update is called once per frame
    void Update()
    {
        if (completed)
        {
            GoalAchieved();
            completed = false;
        }
    }

    void FindClosestGoal(){
        float lessDistance = float.MaxValue;
        nextGoal = null;
        Vector3 playerPosition = player.transform.position;

        foreach (GameObject goal in goals){
             Vector3 goalPosition = goal.transform.position;
             float distance = Vector3.Distance(playerPosition,goalPosition);
             if(distance<lessDistance){
                lessDistance = distance;
                nextGoal = goal;
             }
        }
        if (nextGoal == null)
            {
                goalText.text = "Todos os objetivos encontrados!";
                return;
            }
        string goalName = nextGoal.name.Split(':')[0];
        goalText.text = "Encontre " + goalName;
    }

    void GoalAchieved(){
        goals.Remove(nextGoal);
        FindClosestGoal();
    }

    public void CheckGoalTouch(GameObject touchedObject){
    if (nextGoal == null || completed)
        return;

    if (touchedObject == nextGoal ||
        touchedObject.transform.IsChildOf(nextGoal.transform))
    {
        completed = true;
    }
}
            
}
