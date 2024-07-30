using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTemplates : MonoBehaviour
{
    public GameObject[] bottomRooms;
    public GameObject[] topRooms;
    public GameObject[] leftRooms;
    public GameObject[] rightRooms;
    public int roomsCount;
    
    public List<GameObject> rooms;
    public float waitTime;

    void Start() {
        StartCoroutine(SetRoom());
    }

    IEnumerator SetRoom()
    {   
        while(true)
        {
            waitTime-=Time.deltaTime;
            
            if(waitTime <= 0) break;

            yield return new WaitForFixedUpdate();
        }

        for (int i = 0; i < rooms.Count; i++) {
            rooms[i].GetComponent<Room1>().RoomCheckStart();
        }
    }
}
