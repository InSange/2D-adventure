using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room1 : MonoBehaviour
{
    [SerializeField] GameObject[] ways;
    [SerializeField] GameObject[] spawners;
    [SerializeField] List<Room> nearRooms;
    [SerializeField] RoomCheck[] roomCheckers;
    [SerializeField] RoomTemplates templates;
    public bool clear = false;

    void Start() {
        templates = GameObject.FindGameObjectWithTag("RoomTemplate").GetComponent<RoomTemplates>();
        templates.rooms.Add(this.gameObject);

        // 시작방 길 세팅
        if(gameObject.CompareTag("EntryRoom"))
        {
            int cur_ways = 4;
            for(int i = 0; i < spawners.Length; i++)
            {
                if(cur_ways == 1) 
                {
                    spawners[i].SetActive(true);
                    continue;
                }

                int randWay = Random.Range(0, 4);
                
                if(randWay < 3) 
                {
                    spawners[i].SetActive(true);
                    ways[i].SetActive(true);
                }
                else 
                {
                    cur_ways--;
                    ways[i].SetActive(false);
                }
            }
        }
    }

    public void RoomCheckStart()
    {
        for(int i = 0; i < roomCheckers.Length; i++)
        {
            if(roomCheckers[i].otherTagName == "Rooms" || roomCheckers[i].otherTagName == "EntryRoom") 
            {
                ways[i].SetActive(true);
                nearRooms.Add(roomCheckers[i].otherGameOBJ.GetComponent<Room>());
            }
            else ways[i].SetActive(false);
        }

    }
}
