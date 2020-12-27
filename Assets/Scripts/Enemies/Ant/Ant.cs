using System;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class Ant : EnemyController
{
    [SerializeField] private bool raycast;
    [SerializeField] private bool groundCheck;
    
    protected override void Move()
    {
        base.Move();
        
        if(!raycast && !groundCheck) return;
        
        FindPlayer = Physics2D.OverlapBox(visionPoint.position, new Vector2(rangeOfX, rangeOfY), 0, playerLayer);
        
        IsAngry = FindPlayer;
        
        if (IsAngry)
        {
            if (raycast)
                RaycastCheck();
                
            if (groundCheck)
                GroundCheck();

            if (IsAngry)
            {
                EnemyRb.velocity = new Vector2(transform.right.x * speed, EnemyRb.velocity.y);
            }

            if (playerTransform.position.x - 1f > transform.position.x && !Facing)
                Flip();
                
            if (playerTransform.position.x + 1f < transform.position.x && Facing)
                Flip();
            
            return;
        }
        
        if (raycast)
            RaycastCheck();
        
        if (groundCheck)
            GroundCheck();

        EnemyRb.velocity = new Vector2(transform.right.x * speed, EnemyRb.velocity.y);
    }
    
    protected override void RaycastCheck()
    {
        RaycastHit2D info = Physics2D.Raycast(raycastPosition.transform.position, raycastPosition.transform.right,
            rayLength);
    
        if (info)
        {
            foreach (GameObject o in ignoreObjects)
            {
                if (info.transform.name == o.transform.name)
                    return;
            }
    
            if (IsAngry)
                IsAngry = false;
            else if(!IsAngry)
                Flip();
        }
    }
    
    protected override void GroundCheck()
    {
        if (!isGround() && IsAngry)
            IsAngry = false;
        else if (!isGround() && !IsAngry)
            Flip();
    }
}
