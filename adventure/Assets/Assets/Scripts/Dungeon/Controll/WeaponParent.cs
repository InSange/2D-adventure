using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponParent : MonoBehaviour
{
    public SpriteRenderer characterRenderer, weaponRenderer;
    public Vector2 PointerPosition { get; set; }
    
    public Animator animator;
    [SerializeField] private float delay = 0.5f;
    private bool attackBlocked;

    public bool IsAttacking { get; private set; }

    public Transform circleOrigin;
    public float radius;

    public void Awake()
    {

    }
    
    public void ResetIsAttacking()
    {
        IsAttacking = false;
    }

    private void Update()
    {
        if (IsAttacking)
            return;
        Vector2 direction = (PointerPosition - (Vector2)transform.position).normalized;
        transform.right = direction;
        
        Vector2 scale = transform.localScale;
        if (direction.x < 0)
        {
            scale.y = -1;
        }
        else if (direction.x > 0)
        {
            scale.y = 1;
        }
        transform.localScale = scale;

        if(transform.eulerAngles.z > 0 && transform.eulerAngles.z < 180)
        {
            weaponRenderer.sortingOrder = characterRenderer.sortingOrder - 1;
        }else{
            weaponRenderer.sortingOrder = characterRenderer.sortingOrder + 1;
        }
    }

    public void Attack()
    {
        if (attackBlocked)
            return;
        animator.SetTrigger("Attack");
        IsAttacking = true;
        attackBlocked = true;
        StartCoroutine(DelayAttack());
    }

    private IEnumerator DelayAttack()
    {
        yield return new WaitForSeconds(delay);
        attackBlocked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 position = circleOrigin == null ? Vector3.zero : circleOrigin.position;
        Gizmos.DrawWireSphere(position, radius);
    }

    public void DetectColliders()
    {
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(circleOrigin.position,radius))
        {
            Debug.Log(gameObject.layer + ", " + collider.gameObject.name + ", " + collider.gameObject.layer);
            if(gameObject.layer == 10 && collider.gameObject.layer == 10) continue; // 몬스터일 경우 공격한 오브젝트가 몬스터일 경우 무시
            else if((gameObject.layer == 12|| gameObject.layer == 9) && (collider.gameObject.layer == 12 || collider.gameObject.layer==9)) continue; // 플레이어 또는 팔로워일 경우 공격한 오브젝트가 몬스터가 아니면 무시
            Debug.Log(collider.name);
            Health health;
            if(health = collider.GetComponent<Health>())
            {
                health.GetHit(1, transform.parent.gameObject);
            }
        }
    }
}