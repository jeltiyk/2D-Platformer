using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Game Object")]
    [SerializeField] private GameObject followObject;

    [Header("Settings")]
    [SerializeField] private bool direction = true;     //true == right, false == left
    [SerializeField] private float maxHeightByY = 7.0f;
    [SerializeField] private float minHeightByY = -0.5f;
    [SerializeField] private float smoothing = 3.5f;
    [SerializeField] private float offsetX = 2.0f;
    [SerializeField] private float offsetY = 0.35f;

    public float Smoothing
    {
        get => smoothing;
        set => smoothing = value;
    }
    public GameObject FollowObject
    {
        get => followObject;
        set => followObject = value;
    }

    private Vector2 _offset;
    private float _lastPositionX;
    private float _lastPositionY;
    
    private void Awake()
    {
        if (followObject == null)
        {
            PlayerController findObject = FindObjectOfType<PlayerController>();
            
            if(findObject == null) return;
            
            followObject = findObject.gameObject;
        }

        _offset = new Vector2(offsetX, offsetY);

        if (direction)
            transform.position = new Vector3((followObject.transform.position.x + _offset.x) / 2, followObject.transform.position.y + _offset.y, transform.position.z);
        else
            transform.position = new Vector3((followObject.transform.position.x - _offset.x) / 2, followObject.transform.position.y + _offset.y, transform.position.z);

        _lastPositionX = transform.position.x;
    }
    
    private void LateUpdate()
    {
        FindObject();
    }

    private void FindObject()
    {
        if (followObject != null)
        {
            int currentX = Mathf.RoundToInt(followObject.transform.position.x);

            if (currentX > _lastPositionX)
                direction = true;
            else if (currentX < _lastPositionX)
                direction = false;

            _lastPositionX = currentX;

            Vector3 target;
            
            if (direction)
                target = new Vector3(followObject.transform.position.x + _offset.x, followObject.transform.position.y + _offset.y, transform.position.z);
            else
                target = new Vector3(followObject.transform.position.x - _offset.x, followObject.transform.position.y + _offset.y, transform.position.z);

            if (target.y <= maxHeightByY && target.y >= minHeightByY)
                _lastPositionY = target.y;
            
            transform.position = Vector3.Lerp(transform.position, new Vector3(target.x, _lastPositionY, target.z), smoothing * Time.deltaTime);
        }
        else Debug.Log("Camera: Object not found.");
    }

    public void ChangeObject(GameObject newObject)
    {
        followObject = newObject;
    }

    public void ChangeSmoothing(float newValue)
    {
        smoothing = newValue;
    }
}
