using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NextStage : MonoBehaviour
{
    RoomFirstDungeonGenerator dungeonGenerator;

    // Start is called before the first frame update
    void Start()
    {
        dungeonGenerator = FindObjectOfType<RoomFirstDungeonGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player")) 
        {
            Debug.Log("다음 스테이지로 !");
            dungeonGenerator.NextDungeon();
        }
    }
}
