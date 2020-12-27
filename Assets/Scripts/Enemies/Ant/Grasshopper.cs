using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class Grasshopper : EnemyController
{
    [SerializeField] private bool jump;
     
    protected override void Move()
    {
        base.Move();

        if (!jump) return;
        
        FindPlayer = Physics2D.OverlapBox(visionPoint.position, new Vector2(rangeOfX, rangeOfY), 0, playerLayer);
        
        IsAngry = FindPlayer;

        if (IsAngry)
        {
            // RaycastCheck();

            if (IsAngry)
            {
                StartCoroutine(JumpMoveController());
            }
            
            if (playerTransform.position.x - 1f > transform.position.x && !Facing)
                Flip();
                
            if (playerTransform.position.x + 1f < transform.position.x && Facing)
                Flip();

            return;
        }
        
        StartCoroutine(JumpMoveController());
    }

    protected override void JumpMove()
    {
        RaycastCheck();

        if(!isGround() && !isEnemy()) return;
        
        EnemyRb.velocity = new Vector2(transform.right.x * speed, transform.up.y * Random.Range(jumpForceMinValue, jumpForceMaxValue));
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
    
    private IEnumerator JumpMoveController()
    {
        jump = false;
        
        JumpMove();
        
        if(IsAngry)
            yield return new WaitForSeconds(jumpDelay/2);
        else
            yield return new WaitForSeconds(jumpDelay);

        jump = true;
    }
}
