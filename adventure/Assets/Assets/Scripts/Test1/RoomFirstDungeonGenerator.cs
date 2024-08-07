using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{   
    [SerializeField] private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField] private int dungeonWidth = 20, dungeonHeight = 20;
    
    private Dictionary<Vector2Int, HashSet<Vector2Int>> roomsDictionary = new Dictionary<Vector2Int, HashSet<Vector2Int>>();
    [SerializeField] private DungeonData dungeonData;
    [SerializeField] private RoomDataExtractor roomDataExtractor;

    //private HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>(), corridorPositions = new HashSet<Vector2Int>();

    private List<Color> roomColors = new List<Color>();
    //[SerializeField] private bool showRoomGizmo = false, showCorridorsGizmo;
    
    [SerializeField] [Range(0,10)] private int offset = 1;
    [SerializeField] private bool randomWalkRooms = false;

    private void Awake() {
        RunProceduralGeneration();
    }

    public void NextDungeon()
    {
        RunProceduralGeneration();
    }
    
    protected override void RunProceduralGeneration()
    {
        tilemapVisualizer.Clear();
        dungeonData.Reset();
        CreateRooms();
        roomDataExtractor.ProcessRooms();
    }

    private void CreateRooms()
    {   
        var roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);
        
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        
        if(randomWalkRooms)
        {
            floor = CreateRoomsRandomly(roomsList);
        }
        else  
        {
            floor = CreateSimpleRooms(roomsList);
        }

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach(var room in roomsList)
        {   
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
            //Debug.Log((Vector2Int)Vector3Int.RoundToInt(room.center));
        }
        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        dungeonData.Path.UnionWith(corridors);

        floor.UnionWith(corridors);
        
        tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);

        
    }

    private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> roomsList)
    {   
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        for(int i = 0; i < roomsList.Count; i++)
        {   
            var roomBounds = roomsList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            
            HashSet<Vector2Int> roomDicFloor = new HashSet<Vector2Int>();
           
            foreach(var position in roomFloor)
            {   
                if(position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset) && position.y >= (roomBounds.yMin - offset) && position.y <= (roomBounds.yMax - offset))
                {
                    floor.Add(position);
                    roomDicFloor.Add(position);
                }
            }

            dungeonData.Rooms.Add(new Room(roomCenter, roomDicFloor));
        }
        return floor;
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {   
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while(roomCenters.Count > 0)
        {   
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }

        return corridors;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {   
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        
        corridor.Add(position);
        while(position.y != destination.y)
        {
            if(destination.y > position.y)
            {
                position += Vector2Int.up;
            }
            else if(destination.y < position.y)
            {
                position += Vector2Int.down;
            }
            corridor.Add(position);
            corridor.Add(position+Vector2Int.left);
            corridor.Add(position+Vector2Int.right);
        }
        while(position.x != destination.x)
        {
            if(destination.x > position.x)
            {
                position += Vector2Int.right;
            }
            else if(destination.x < position.x)
            {
                position += Vector2Int.left;
            }
            corridor.Add(position);
            corridor.Add(position+Vector2Int.up);
            corridor.Add(position+Vector2Int.down);
        }
        return corridor;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    { 
        Vector2Int closest = Vector2Int.zero;
        float length = float.MaxValue;

        foreach(var position in roomCenters)
        {
            float currentDistance = Vector2.Distance(position, currentRoomCenter);
            if(currentDistance < length)
            {
                length = currentDistance;
                closest = position;
            }
        }
        return closest;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {  
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach(var room in roomsList)
        {
            for(int col = offset; col < room.size.x - offset; col++)
            {
                for(int row = offset; row < room.size.y - offset; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
        }
        return floor;
    }
}   
