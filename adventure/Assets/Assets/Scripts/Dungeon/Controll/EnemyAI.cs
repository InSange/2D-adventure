using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyAI : MonoBehaviour
{
    public UnityEvent<Vector2> OnMovementInput, OnPointerInput;
    public UnityEvent OnAttack;

    // [SerializeField]
    // private Transform player;

    // [SerializeField]
    // private float chaseDistanceThreshold = 3, attackDistanceThreshold = 0.8f;
    // private float passedTime = 1;

    [SerializeField]
    private List<SteeringBehaviour> steeringBehaviours;

    [SerializeField]
    private List<Detector> detectors;

    [SerializeField]
    private AIData aiData;

    [SerializeField]
    private float detectionDelay = 0.05f, aiUpdateDelay = 0.06f, attackDelay = 1f;

    [SerializeField]
    private float attackDistance = 1.0f;

    [SerializeField]
    private Vector2 movementInput;

    [SerializeField]
    private ContextSolver movementDirectionSolver;

    bool following = false;

    private void Start()
    {
        //Detecting Player and Obstacles around
        InvokeRepeating("PerformDetection", 0.2f, detectionDelay);
    }

    private void PerformDetection()
    {
        foreach (Detector detector in detectors)
        {
            detector.Detect(aiData);
        }
    }

    private void Update()
    {
        //Enemy AI movement based on Target availability
        if (aiData.currentTarget != null)
        {
            //Looking at the Target
            OnPointerInput?.Invoke(aiData.currentTarget.position);
            if (following == false)
            {
                following = true;
                StartCoroutine(ChaseAndAttack());
            }
        }
        else if (aiData.GetTargetsCount() > 0)
        {
            aiData.currentTarget = aiData.targets[0];
            Debug.Log("감지 수 " + aiData.targets.Count);
        }
        //Moving the Agent
        OnMovementInput?.Invoke(movementInput);
    }

    private IEnumerator ChaseAndAttack()
    {
        if (aiData.currentTarget == null)
        {
            //Stopping Logic
            Debug.Log("Stopping");
            movementInput = Vector2.zero;
            following = false;
            yield break;
        }
        else
        {
            float distance = Vector2.Distance(aiData.currentTarget.position, transform.position);

            if (distance < attackDistance)
            {   
                //Attack logic
                movementInput = Vector2.zero;
                OnAttack?.Invoke();
                yield return new WaitForSeconds(attackDelay);
                StartCoroutine(ChaseAndAttack());
            }
            else
            {
                //Chase logic
                movementInput = movementDirectionSolver.GetDirectionToMove(steeringBehaviours, aiData);
                yield return new WaitForSeconds(aiUpdateDelay);
                StartCoroutine(ChaseAndAttack());
            }

        }

    }

    
    // private void Update() {
    //     if(player == null) return;

    //     float distance = Vector2.Distance(player.position, transform.position);
    //     if(distance < chaseDistanceThreshold)
    //     {
    //         OnPointerInput?.Invoke(player.position);

    //         if(distance <= attackDistanceThreshold)
    //         {
    //             Debug.Log("공격");
    //             //공격
    //             OnMovementInput?.Invoke(Vector2.zero);
    //             if(passedTime >= attackDelay)
    //             {
    //                 passedTime = 0;
    //                 OnAttack?.Invoke();
    //             }
    //         }
    //         else
    //         {
    //             Debug.Log("추격");
    //             //추적
    //             Vector2 direction = player.position - transform.position;
    //             OnMovementInput?.Invoke(direction.normalized);
    //         }
    //     }
    //     else
    //     {
    //         OnMovementInput?.Invoke(Vector2.zero);
    //     }
    //     //대기 상태(공격을 할 수 없는 상태일 때)
    //     if(passedTime < attackDelay)
    //     {
    //         passedTime += Time.deltaTime;
    //     }
    // }
}
