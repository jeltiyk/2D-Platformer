using UnityEngine;

public class SceneEndController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController>();
        if(player == null || !player.ToNextLevel || SceneController.Instance.ExistsEndCutscene) return;
        
        SceneController.Instance.EndScene();
    }
}
