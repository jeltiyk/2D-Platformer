using UnityEngine;

public class KeyController : MonoBehaviour
{
    [SerializeField] private bool toNextLevel;
    private bool _collected;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController>();

        if(!toNextLevel || _collected || player == null) return;

        _collected = true;
        
        player.OnChangeLevelState(true);
        
        Destroy(gameObject);
    }
}
