using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Agent : MonoBehaviour
{
    private AgentAnimations agentAnimations;
    private AgentMover agentMover;

    //[SerializeField] private InputActionReference movement, attack, pointerPosition;

    private WeaponParent weaponParent;

    private Vector2 pointerInput, movementInput;

    public Vector2 PointerInput { get => pointerInput; set => pointerInput = value; }
    public Vector2 MovementInput { get => movementInput; set => movementInput = value; }

    // private void OnEnable()
    // {
    //     attack.action.performed += PerformAttack;
    // }

    // private void OnDisable()
    // {
    //     attack.action.performed -= PerformAttack;
    // }

    public void PerformAttack()
    {
        weaponParent.Attack();
    }

    private void Awake()
    {
        agentAnimations = GetComponentInChildren<AgentAnimations>();
        weaponParent = GetComponentInChildren<WeaponParent>();
        agentMover = GetComponent<AgentMover>();
    }

    private void AnimateCharacter()
    {
        Vector2 lookDirection = PointerInput - (Vector2)transform.position;
        agentAnimations.RotateToPointer(lookDirection);
        agentAnimations.PlayAnimation(MovementInput);
    }

    private void Update()
    {
        //pointerInput = GetPointerInput();
        weaponParent.PointerPosition = PointerInput;
        //movementInput = movement.action.ReadValue<Vector2>().normalized;

        agentMover.MovementInput = MovementInput;
        
        AnimateCharacter();
    }

    // private Vector2 GetPointerInput()
    // {   
    //     Vector3 mousePos = Mouse.current.position.ReadValue(); //pointerPosition.action.ReadValue<Vector2>();
    //     mousePos.z = Camera.main.nearClipPlane;
    //     return Camera.main.ScreenToWorldPoint(mousePos);
    // }

}