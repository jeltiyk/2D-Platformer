using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class Ant : EnemyController
{
    private void Start()
    {
        EnemyRb.mass = 10f;
    }
    
    private void OnCollisionStay2D(Collision2D other)
    {
        if(other.collider.GetComponent<PlayerController>() != null) 
            HitsCollider.enabled = false;
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if(other.collider.GetComponent<PlayerController>() != null)
            HitsCollider.enabled = true;
    }
}
