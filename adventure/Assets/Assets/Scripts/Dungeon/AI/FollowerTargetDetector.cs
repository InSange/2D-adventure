using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FollowerTargetDetector : Detector
{
    [SerializeField]
    private float targetDetectionRange = 5, playerDetectionRange = 100;

    [SerializeField]
    private LayerMask obstaclesLayerMask, EnemyLayerMask, PlayerLayerMask;

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
        //         //Debug.Log(hit.collider.name + "�� �浹����");
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

        Collider2D[] EnemyCollider = Physics2D.OverlapCircleAll(transform.position, targetDetectionRange, EnemyLayerMask);

        if (EnemyCollider != null)
        {   
            colliders = new List<Transform>();
            
            for(int i = 0; i < EnemyCollider.Length; i++)
            {
                colliders.Add(EnemyCollider[i].transform);
            }
        }
        else
        {   
            //Enemy doesn't see the player
            //colliders = null;
            Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, playerDetectionRange, PlayerLayerMask);

            colliders = new List<Transform>() {playerCollider.transform};
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