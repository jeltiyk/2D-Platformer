using UnityEngine;

public class CoinController : MonoBehaviour
{
    [SerializeField] private int coinScore;
    private bool _collected;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController>();
        if(_collected || player == null) return;
        
        _collected = true;
        
        player.OnChangeCoins(coinScore);
        Destroy(gameObject);
    }
}
