using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SimpleRandomWalkParameters_",menuName = "PCG/SimpleRandomWalkData")]
public class SimpleRandomWalkSO : ScriptableObject
{   // ScriptableObject로 아래의 기본 값들을 지니는 에셋형식으로 저장한다.
    public int iterations = 10, walkLength = 10;
    public bool startRandomlyEachIteration = false;
}
