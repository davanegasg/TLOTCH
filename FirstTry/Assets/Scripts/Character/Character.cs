using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Character : MonoBehaviour
{
    CharacterAnimator animator;
    public float moveSpeed;
    public bool IsMoving { get; private set; }
    public float OffsetY { get; private set; } = 0.3f;
    private void Awake()
    {
        animator = GetComponent<CharacterAnimator>();
        SetPositionAndSnapToTile(transform.position);
    }
    public IEnumerator Move(Vector2 moveVec, Action OnMoveOver= null)
    {
        animator.MoveX = moveVec.x;
        animator.MoveY = moveVec.y;
        var targetPos = transform.position;
        targetPos.x += moveVec.x;
        targetPos.y += moveVec.y;

        if (!isWalkable(targetPos))
            yield break;

        IsMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        IsMoving = false;

        
        OnMoveOver?.Invoke();
    }
    public void SetPositionAndSnapToTile(Vector2 pos)
    {
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f+OffsetY;

        transform.position = pos;
    }
    public void HandleUpdate()
    {
        animator.IsMoving = IsMoving;
    }
    private bool isWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos - new Vector3(0, OffsetY), 0.2f, GameLayers.i.SolidLayer) != null)
        {
            return false;
        }
        return true;
    }

    public CharacterAnimator Animator
    {
        get => animator;
    }
}
