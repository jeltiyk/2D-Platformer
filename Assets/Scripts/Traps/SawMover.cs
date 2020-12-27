using UnityEngine;

public class SawMover : MonoBehaviour
{
    [SerializeField] private float speed = 2;
    [SerializeField] private float range = 3.8f;
    [SerializeField] private int direction = 1;

    private Vector2 _startPoint;

    private void Start()
    {
        _startPoint = transform.position; 
    }

    private void FixedUpdate()
    {
        ChangeDirection();
        transform.Translate(speed * direction * Time.deltaTime, 0, 0);
    }

    private void ChangeDirection()
    {
        if (transform.position.x - _startPoint.x > range && direction > 0)
            direction *= -1;
        else if (transform.position.x < _startPoint.x && direction < 0)
            direction *= -1;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + range, transform.position.y, transform.position.z));
        //Gizmos.DrawWireCube(transform.position, new Vector3(range, 0.5f, transform.position.z));
    }
}
