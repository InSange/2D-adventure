using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AnimationController : ScriptableObject
{
    public List<AnimatorOverrideController> animaotrs = new List<AnimatorOverrideController>();
}
