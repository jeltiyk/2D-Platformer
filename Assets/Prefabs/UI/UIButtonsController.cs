using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class UIButtonsController : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    
    private Component[] _components;
    private void Awake()
    {
        _components = GetComponentsInChildren(typeof(Button), true);

        if (_components == null) return;
        
        foreach (Button button in _components)
        {
            button.onClick.AddListener(PlaySound);
        }
    }

    private void OnDestroy()
    {
        if(_components == null) return;

        foreach (Button button in _components)
        {
            button.onClick.RemoveListener(PlaySound);
        }
    }

    private void PlaySound()
    {
        source.Play();
    }
}
