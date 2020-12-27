using UnityEngine;

public class SuperAcornController : MonoBehaviour
{
    [SerializeField] private int heal = 100;
    [SerializeField] private int pineConesCount = 15;
    [SerializeField] private int coinScore = 10;
    private bool _collected;
    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController>();
        if(player == null || _collected) return;
        
        _collected = true;
        
        var emissionModule = GetComponentInChildren<ParticleSystem>().emission;
        emissionModule.enabled = false;

        #region Legacy

        // for (int i = 0; i < transform.childCount; i++)
        // {
        //     if(transform.GetChild(i).name != "Acorn") continue;
        //     
        //     Transform acorn = transform.GetChild(i);
        //     
        //     player.OnChangeHealth(heal);
        //     player.OnChangePineCones(pineConesCount);
        //     player.OnChangeCoins(coinScore);
        //     
        //     Debug.Log("LOG");
        //     
        //     Destroy(acorn.gameObject);
        // }

        #endregion

        Transform acorn = transform.GetChild(0);

        player.OnChangeHealth(heal);
        player.OnChangePineCones(pineConesCount);
        player.OnChangeCoins(coinScore);
        
        Destroy(acorn.gameObject);
        Destroy(gameObject, 3f);
    }
}
