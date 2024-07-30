using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCheck : MonoBehaviour
{
    public string otherTagName;
    public GameObject otherGameOBJ;
    void OnTriggerEnter(Collider other){
        if(other.CompareTag("Rooms") || other.CompareTag("EntryRoom")) 
        {
            otherTagName = other.tag;
            otherGameOBJ = other.gameObject;
        }
	}
}
