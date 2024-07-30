using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FollwerAI : MonoBehaviour
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
    private float attackDistance = 1.0f, followDistance = 2.0f, teleportDistance = 10.0f;

    [SerializeField]
    private Vector2 movementInput;

    [SerializeField]
    private ContextSolver movementDirectionSolver;

    [SerializeField]
    private Transform followerTarget;

    [SerializeField] bool following = false;
    [SerializeField] private Vector3 beforePos;
    [SerializeField] int telposCnt = 0;

    private void Start()
    {
        followerTarget = FindObjectOfType<PlayerInput>().transform;

        //Detecting Player and Obstacles around
        InvokeRepeating("PerformDetection", 0, detectionDelay);
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
            aiData.currentTarget = aiData.targets[0];

            //Looking at the Target
            OnPointerInput?.Invoke(aiData.currentTarget.position);
            if (following == false)
            {
                following = true;
                Debug.Log("팔로워 따라다닌다 ");
                StartCoroutine(ChaseAndAttack());
            }
        }
        else if (aiData.GetTargetsCount() > 0)
        {
            //Target acquisition logic
            aiData.currentTarget = aiData.targets[0];
        }
        else
        {
            OnPointerInput?.Invoke(followerTarget.position);
            StartCoroutine(FollowPlayer());
        }

        //Moving the Agent
        OnMovementInput?.Invoke(movementInput);
    }

    private IEnumerator FollowPlayer()
    {
        float distance = Vector2.Distance(followerTarget.position, transform.position);

        if (distance < followDistance)
        {   
            movementInput = Vector2.zero;
            yield return new WaitForSeconds(attackDelay);
            StartCoroutine(ChaseAndAttack());
        }
        else if(distance >= teleportDistance)
        {
            transform.position = followerTarget.position;
            yield return new WaitForSeconds(aiUpdateDelay);
        }
        else
        {
            //Chase logic
            movementInput = (followerTarget.position - transform.position).normalized;
            yield return new WaitForSeconds(aiUpdateDelay);
            if(beforePos == null) beforePos = transform.position;
            else
            {
                if(beforePos == transform.position)
                {
                    telposCnt++;
                }
                else
                {
                    beforePos = transform.position;
                    telposCnt = 0;
                }
                
                if(telposCnt == 100)
                {
                    transform.position = followerTarget.position;
                    telposCnt = 0;
                }
            }
        }
    }

    private IEnumerator ChaseAndAttack()
    {
        if (aiData.currentTarget == null)
        {
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
                Debug.Log("여기?");
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
