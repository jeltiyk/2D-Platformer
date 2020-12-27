using UnityEngine;

public class AcornController : MonoBehaviour
{
    [SerializeField] private int heal;
    private bool _collected;
    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController>();

        if (_collected || player == null) return;

        _collected = true;

        player.OnChangeHealth(heal);
        Destroy(gameObject);
    }
}
