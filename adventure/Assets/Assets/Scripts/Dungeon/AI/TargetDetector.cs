using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TargetDetector : Detector
{
    [SerializeField]
    private float targetDetectionRange = 5;

    [SerializeField]
    private LayerMask obstaclesLayerMask, playerLayerMask;

    [SerializeField]
    private bool showGizmos = false;

    //gizmo parameters
    private List<Transform> colliders;

    public override void Detect(AIData aiData)
    {
        //Find out if player is near
        // Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, targetDetectionRange, playerLayerMask);

        // if (playerCollider != null)
        // {   
        //     //Check if you see the player
        //     Vector2 direction = (playerCollider.transform.position - transform.position).normalized;
        //     RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, targetDetectionRange, obstaclesLayerMask);

        //     //Make sure that the collider we see is on the "Player" layer
        //     if (hit.collider != null && ((playerLayerMask & (1 << hit.collider.gameObject.layer)) != 0 ))
        //     {   
        //         //Debug.Log(hit.collider.name + "이 충돌했음");
        //         Debug.DrawRay(transform.position, direction * targetDetectionRange, Color.magenta);
        //         colliders = new List<Transform>() { playerCollider.transform };
        //     }
        //     else
        //     {  
        //         colliders = null;
        //     }
        // }
        // else
        // {   
        //     //Enemy doesn't see the player
        //     colliders = null;
        // }

        Collider2D[] playerCollider = Physics2D.OverlapCircleAll(transform.position, targetDetectionRange, playerLayerMask);

        if (playerCollider != null)
        {   
            colliders = new List<Transform>();
            
            for(int i = 0; i < playerCollider.Length; i++)
            {
                Vector2 direction = (playerCollider[i].transform.position - transform.position).normalized;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, targetDetectionRange, obstaclesLayerMask);

                //Make sure that the collider we see is on the "Player" layer
                if (hit.collider != null && ((playerLayerMask & (1 << hit.collider.gameObject.layer)) != 0 ))
                {   
                    //Debug.Log(hit.collider.name + "이 충돌했음");
                    Debug.DrawRay(transform.position, direction * targetDetectionRange, Color.magenta);
                    colliders = new List<Transform>() { playerCollider[i].transform };
                }
                colliders.Add(playerCollider[i].transform);
            }
            if(colliders.Count == 0) colliders = null;
        }
        else
        {   
            //Enemy doesn't see the player
            colliders = null;
        }

        aiData.targets = colliders;
    }

    private void OnDrawGizmosSelected()
    {
        if (showGizmos == false)
            return;

        Gizmos.DrawWireSphere(transform.position, targetDetectionRange);

        if (colliders == null)
            return;
        Gizmos.color = Color.magenta;
        foreach (var item in colliders)
        {
            Gizmos.DrawSphere(item.position, 0.3f);
        }
    }
}