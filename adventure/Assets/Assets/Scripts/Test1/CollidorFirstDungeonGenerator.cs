using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CollidorFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{   // 복도의 길이와 개수를 임의로 지정해주었다.
    [SerializeField] private int corridorLength = 14, corridorCount = 5;
    // 방의 비율 값이다.
    [SerializeField] [Range(0.1f, 1)] private float roomPercent = 0.8f;
    // 오버라이드로 기존에 맵만 만들어지는 것에서 복도까지 추가적으로 생성되게끔 덮어쓴다.
    protected override void RunProceduralGeneration()
    {
        CollidorFirstGeneration();
    }
    // 던전의 방과 복도를 만드는 함수이다.
    private void CollidorFirstGeneration()
    {   // 던전이 만들어질 벡터값들을 저장하는 변수들이다.
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();
        // 복도를 만드는 함수이다.
        List<List<Vector2Int>> corridors = CreateCorridors(floorPositions, potentialRoomPositions);
        // 복도를 만들면서 생성해준 방의 위치값들을 전달하여 집합으로 묶어준다.
        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);
        // 우리는 복도의 끝부분들 중 일부분에만 방을 생성했다. 
        // 나머지 복도들중 복도 끝에 방은 없으나 방만 존재하는 문제점이 있다.
        // 그 문제점을 해결하기위해 복도는 존재하지만 끝에 방이 없는 지점을 찾기위해 리스트를 만들었다.
        List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);
        // 끝 지점을 찾아주는 함수.
        CreateRoomsAtDeadEnd(deadEnds, roomPositions);
        // 이후에 생성된 방까지 집합에 넣어준다.
        floorPositions.UnionWith(roomPositions);

        for(int i = 0; i < corridors.Count; i++)
        {
            //corridors[i] = IncreaseCorridorSizeByOne(corridors[i]);
            corridors[i] = IncreaseCorridorBrush3by3(corridors[i]);
            floorPositions.UnionWith(corridors[i]);
        }

        // 그려준다.
        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    }

    private void CreateRoomsAtDeadEnd(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomFloors)
    {   // 끝 지점에 위치한 복도들 중 방이 존재하지 않는다면 방을 생성해준다.
        foreach (var position in deadEnds)
        {
            if(roomFloors.Contains(position) == false)
            {
                var room = RunRandomWalk(randomWalkParameters, position);
                roomFloors.UnionWith(room);
            }
        }
    }

    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions)
    {   // 방이 없는 끝 지점 복도의 벡터값들을 저장해둘 리스트이다.
        List<Vector2Int> deadEnds = new List<Vector2Int>();
        // 모든 복도의 벡터값들은 floorPositions에 저장되어있다.
        foreach(var position in floorPositions)
        {   // 상,하,좌,우 인접한 위치에 타일(땅, 복도, 벽)이 존재할시에 neighbourCount를 증가시킨다.
            int neighboursCount = 0;
            foreach(var direction in Direction2D.cardinalDirectionsList)
            {   
                if(floorPositions.Contains(position + direction))
                {
                    neighboursCount++;
                }
            }
            Debug.Log(position + "의 개수 " + neighboursCount);
            // 근처에 타일이 1개가 존재하는 것은 방이 없고 복도만 존재하는 복도 끝부분이다.
            // 방이 존재한다면 복도로 연결되어있는 타일 1개 + 방의 타일 n개로써 1개 이상일 수 밖에없다.
            if(neighboursCount == 1)
            {   // 끝 지점만 존재하는 복도 위치를 넣어준다.
                deadEnds.Add(position);
            }
        }
        return deadEnds;
    }
    // 이전에 생성한 위치값들을 기반으로 방을 만들어줄 함수이다.
    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {   // 복도 끝부분에서 넓히며 퍼져나간 방의 위치값들을 저장해줄 집합 셋이다.
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        // 복도 끝부분들 중에서 roomPercent의 비율만큼 곱해 Mathf.RoundToInt를 통해 첫재짜리에서 반올림해주었다.
        int roomToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count*roomPercent);
        // Shuffle과 같다. 방의 위치값들을 Guid.NewGuid()로 생성된 x값에 따라 정렬후 roomToCreateCount 개수만큼 빼와서 리스트로 만들어준다.
        // 방의 개수를 조정해줌. => 모든 위치값을 방으로 만드는게 아닌 일부만 가져와서 만든다.
        List<Vector2Int> roomToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();
        // 방을 만들기위해 방의 토대가 되는 위치값들을 바탕으로 랜덤한 방향으로 퍼져서 면적을 이룬다.
        foreach(var roomPosition in roomToCreate)  
        {   // roomPosition의 위치를 토대로 방을 생성.
            var roomFloor = RunRandomWalk(randomWalkParameters, roomPosition);
            // 방에 해당하는 모든 위치값들을 roomPositions에 넣어준다.
            roomPositions.UnionWith(roomFloor);
        }
        Debug.Log("방의 개수 " + roomToCreateCount);
        return roomPositions;
    }
    // 복도 생성 함수.
    private List<List<Vector2Int>> CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions)
    {   // 시작 지점부터 방이 만들어질 수 있는 벡터 값들을 추가한다.
        var currentPosition = startPosition;
        potentialRoomPositions.Add(currentPosition);

        List<List<Vector2Int>> corridors = new List<List<Vector2Int>>();

        // 만들고자하는 복도의 개수만큼 복도를 생성해준다.
        for(int i = 0; i < corridorCount; i++)
        {   // 복도위치값들을 생성하는 알고리즘을 현재위치부터 길이까지 전달하여 실행.
            var corridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength);
            
            corridors.Add(corridor);
            // 현재 위치를 생성된 복도 위치값들(리스트로 저장됨)중 마지막에 저장된 값으로 설정한다.
            currentPosition = corridor[corridor.Count-1];
            // 즉 복도 끝 부분에 방이 생성될 가능성이 있기때문에 복도 끝 위치값을 추가해준다.
            potentialRoomPositions.Add(currentPosition);
            // 생성된 위치 값들은 그려주기 위해서 floorPosition 집합에 넣어준다.
            floorPositions.UnionWith(corridor);
        }

        return corridors;
    }

    private List<Vector2Int> IncreaseCorridorBrush3by3(List<Vector2Int> corridor)
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();
        for(int i = 1; i < corridor.Count; i++)
        {
            for(int x = -1; x < 2; x++)
            {
                for(int y = -1; y < 2; y++)
                {
                    newCorridor.Add(corridor[i-1] + new Vector2Int(x, y));
                }
            }
        }
        return newCorridor;
    }

    public List<Vector2Int> IncreaseCorridorSizeByOne(List<Vector2Int> corridor) 
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();
        Vector2Int previousDirection = Vector2Int.zero;

        for(int i = 1; i < corridor.Count; i++)
        {
            Vector2Int directionFromCell = corridor[i] - corridor[i - 1];
            if(previousDirection != Vector2Int.zero && directionFromCell != previousDirection)
            {
                for(int x = -1; x < 2; x++)
                {
                    for(int y = -1; y < 2; y++)
                    {
                        newCorridor.Add(corridor[i-1] + new Vector2Int(x,y));
                    }
                }
                previousDirection = directionFromCell;
            }
            else
            {
                Vector2Int newCorridorTileOffset = GetDirection90From(directionFromCell);
                newCorridor.Add(corridor[i-1]);
                newCorridor.Add(corridor[i-1] + newCorridorTileOffset);
            }
        }
        return newCorridor;
    }

    private Vector2Int GetDirection90From(Vector2Int direction)
    {
        if(direction == Vector2Int.up) return Vector2Int.right;
        if(direction == Vector2Int.right) return Vector2Int.down;
        if(direction == Vector2Int.down) return Vector2Int.left;
        if(direction == Vector2Int.left) return Vector2Int.up;

        return Vector2Int.zero;
    }
}
