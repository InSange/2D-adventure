using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class ProceduralGenerationAlgorithms
{
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLength)
    {   // Hash 집합으로 Vector인자를 저장. 중복 허용X 
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();
        // 시작 지점을 먼저 추가한다.
        path.Add(startPosition);
        var previousposition = startPosition;
        // WalkLength만큼 실행하여 이전 위치에서 랜덤한 위치를 더해 path에 추가한다.
        for (int i = 0; i < walkLength; i++)
        {
            var newPosition = previousposition + Direction2D.GetRandomCardinalDirection();
            path.Add(newPosition);
            previousposition = newPosition;
        }
        // 만들어진 경로들을 반환한다.
        return path;
    }
    // 복도 만들기
    public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPosition, int corriderLength)
    {   // 복도 위치값을 저장하는 리스트 하나를 생성한다.
        List<Vector2Int> corridor = new List<Vector2Int>();
        // 복도는 일자 방향을 유지하기 위해서 위, 아래, 왼쪽, 오른쪽 중 하나의 벡터값을 랜덤으로 가져온다.
        var direction = Direction2D.GetRandomCardinalDirection();
        // 시작 포지션부터 추가해나간다.
        var currentPosition = startPosition;
        corridor.Add(currentPosition);
        // 파라미터로 가져온 복도 길이만큼 direction 방향으로 쭉 늘려준다.
        for(int i = 0; i < corriderLength; i++)
        {
            currentPosition += direction;
            corridor.Add(currentPosition);
        }
        // 만들어진 복도를 반환한다!
        return corridor;
    }

    // BoundsInt 경계상자 값들을 바탕으로 공간을 나누었다. 파라미터로 나눌 경계상자 값들과 최소 너비와 높이를 전달해주었다.
    public static List<BoundsInt>BinarySpacePartitioning(BoundsInt spaceToSplit, int minWidth, int minHeight)
    {   // 모든 공간들에 대해서 쪼개고 쪼개는 분할 방법으로 접근할 것이기에 큐를 하나 생성해주었다.
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();
        // 쪼개진 모든 공간들은 roomsList에 넣어서 반환해 줄 것이다.
        List<BoundsInt> roomsList = new List<BoundsInt>();
        // 첫 공간으로 파라미터 spaceToSplit값을 전달해 주었다.
        roomsQueue.Enqueue(spaceToSplit);
        // 쪼개야할 공간들이 큐에 존재한다면 무한 반복.
        while(roomsQueue.Count > 0)
        {   // 큐의 맨 앞에 있는 공간을 가져온다.
            var room = roomsQueue.Dequeue();
            // 공간안에는 던전 방이 들어가기 때문에 최소한의 충분한크기의 공간이 존재해야한다.
            if(room.size.y >= minHeight && room.size.x >= minWidth)
            {   // 랜덤값을 통해 50%확률로 수평으로 나눌 것인지, 수평으로 나눌 것인지 정한다.
                if(Random.value < 0.5f)
                {   // 높이 또는 너비가 최소값보다 두배 이상 클 경우 쪼개준다. 그렇지 않다면 방을 만들 공간이 준비되었다.
                    if(room.size.y >= minHeight * 2)
                    {
                        SplitHorizontally(minHeight, roomsQueue, room);
                    }
                    else if(room.size.x >= minWidth*2)
                    {
                        SplitVertically(minHeight, roomsQueue, room);
                    }
                    else if(room.size.x >= minWidth && room.size.y >= minHeight)
                    {   // 방이 생성될 공간 추가
                        roomsList.Add(room);
                    }
                }
                else
                {   // 높이 또는 너비가 최소값보다 두배 이상 클 경우 쪼개준다. 그렇지 않다면 방을 만들 공간이 준비되었다.
                    if(room.size.x >= minWidth*2)
                    {
                        SplitVertically(minHeight, roomsQueue, room);
                    }
                    else if(room.size.y >= minHeight * 2)
                    {
                        SplitHorizontally(minWidth, roomsQueue, room);
                    }
                    else if(room.size.x >= minWidth && room.size.y >= minHeight)
                    {   // 방이 생성될 공간 추가
                        roomsList.Add(room);
                    }
                }
            }
        }
        // 모든 공간 분할 완료
        return roomsList;
    }

    private static void SplitVertically(int minHeight, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {   // 전달 받은 room을 xSplit의 크기만큼 나누어 2개의 공간을 만든다.
        // room의 맨 왼쪽 값 room.min에서 room.min + xSplit 까지 하나, room.min+xSplit에서 나머지 x크기 까지 하나를 만든다.
        var xSplit = Random.Range(1, room.size.x);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z), new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));
        // 생성된 방들을 큐에 추가.
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    private static void SplitHorizontally(int minWidth, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        // 전달 받은 room을 ySplit의 크기만큼 나누어 2개의 공간을 만든다.
        // room의 맨 왼쪽 값 room.min에서 room.min + ySplit 까지 하나, room.min+ySplit에서 나머지 y크기 까지 하나를 만든다.
        var ySplit = Random.Range(1, room.size.y);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z), new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));
        // 생성된 방들을 큐에 추가.
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }
}

public static class Direction2D
{
    // 직선 4방향
    public static List<Vector2Int> cardinalDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0, 1), // 위
        new Vector2Int(1, 0), // 오른쪽
        new Vector2Int(0, -1), // 아래
        new Vector2Int(-1, 0) // 왼쪽
    };
    // 대각선 4방향
    public static List<Vector2Int> diagonalDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(1, 1), // 위-오른쪽
        new Vector2Int(1, -1), // 오른쪽-아래
        new Vector2Int(-1, -1), // 아래 - 왼쪽
        new Vector2Int(-1, 1) // 왼쪽 - 위
    };
    // 8방향 (대각선 + 직선)
    public static List<Vector2Int> eightDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0, 1), // 위
        new Vector2Int(1, 1), // 위-오른쪽
        new Vector2Int(1, 0), // 오른쪽
        new Vector2Int(1, -1), // 오른쪽-아래
        new Vector2Int(0, -1), // 아래
        new Vector2Int(-1, -1), // 아래 - 왼쪽
        new Vector2Int(-1, 0), // 왼쪽
        new Vector2Int(-1, 1) // 왼쪽 - 위
    };

    public static Vector2Int GetRandomCardinalDirection()
    {   // 위의 리스트에서 랜덤한 값을 추출.
        return cardinalDirectionsList[Random.Range(0, cardinalDirectionsList.Count)];
    }
}