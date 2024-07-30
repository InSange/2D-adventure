using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

public class SimpleRandomWalkDungeonGenerator : AbstractDungeonGenerator
{   // 미리 저장해놓은 Scriptable 데이터를 가져온다.
    [SerializeField] protected SimpleRandomWalkSO randomWalkParameters;
    
    protected override void RunProceduralGeneration()
    {   // 랜덤한 위치를 저장해놓을 집합소.
        HashSet<Vector2Int> floorPositions = RunRandomWalk(randomWalkParameters, startPosition);
        foreach(var position in floorPositions)
        {
            Debug.Log(position);
        }
        // 집합소에 저장해놓은 위치들을 타일로 그려줌.
        tilemapVisualizer.Clear();
        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    }

    protected HashSet<Vector2Int> RunRandomWalk(SimpleRandomWalkSO randomWalkParameters, Vector2Int position)
    {
        // 현재 위치를 처음 시작 포지션으로 설정
        var currentPosition = position;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();

        for(int i = 0; i < randomWalkParameters.iterations; i++)
        {   // 절차적 생성 알고리즘을 실행. (현재 )
            var path = ProceduralGenerationAlgorithms.SimpleRandomWalk(currentPosition, randomWalkParameters.walkLength);
            // 반복적으로 생성된 path들을 floorPositions에 추가한다.
            floorPositions.UnionWith(path);
            // 다양한 길들을 생성하기위해 floorPositions안에 있는 vector들중 하나를 랜덤으로 선택.
            if(randomWalkParameters.startRandomlyEachIteration)
            {
                currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
            }
        }
        // 생성된 위치들을 반환.
        return floorPositions;
    }
}
