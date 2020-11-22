using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] private int heal = 25;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController>();

        if (player == null) return;
        
        player.OnChangeHealth(heal);
        Destroy(gameObject);
    }
}
