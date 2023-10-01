using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassRender : MonoBehaviour
{
    public float distance = 10;
    GameObject player;
    GameObject[] childs;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        childs = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            childs[i] = transform.GetChild(i).gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject child in childs)
        {
            if (Vector3.Distance(player.transform.position, child.transform.position) < distance)
            {
                child.GetComponent<SpriteRenderer>().enabled = true;
            }
            else
            {
                child.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }
}
