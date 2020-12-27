using UnityEngine;

public class ShopController : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    private GameObject _middlePanel;
    private ShopMenuController _shopMenu;
    private bool _inRange;

    private void Start()
    {
        Component[] components;
        
        if (canvas != null)
        {
            components = canvas.GetComponentsInChildren<Component>(true);

            foreach (var component in components)
            {
                if(component.transform.name != "Middle Panels") continue;

                _middlePanel = component.gameObject;
                return;
            }
        }
        
        components = FindObjectOfType<Canvas>().GetComponentsInChildren<Component>(true);
    
        foreach (var component in components)
        {
            if(component.transform.name != "Middle Panels") continue;

            _middlePanel = component.gameObject;
            Debug.Log(component.transform.name);
            return;
        }
    }

    private void LateUpdate()
    {
        if((Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.T)) && _inRange)
            _middlePanel.gameObject.SetActive(!_middlePanel.gameObject.activeSelf);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController>();
        if(player == null) return;

        _inRange = true;
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController>();
        if(player == null) return;

        _inRange = false;
        _middlePanel.SetActive(false);
    }
}
