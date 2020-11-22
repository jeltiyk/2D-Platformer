using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Game Object")]
    [SerializeField] private GameObject followObject;

    [Header("Settings")]
    [SerializeField] private bool direction = true;     //true == right, false == left
    [SerializeField] private float smoothing = 3.5f;
    [SerializeField] private float offsetX = 2.0f;
    [SerializeField] private float offsetY = 0.5f;

    private Vector2 _offset;
    private int _lastPositionX;

    private void Start()
    {
        _offset = new Vector2(offsetX, offsetY);
        _lastPositionX = Mathf.RoundToInt(followObject.transform.position.x);

        if (direction)
            transform.position = new Vector3((followObject.transform.position.x + _offset.x) / 2, followObject.transform.position.y + _offset.y, transform.position.z);
        else
            transform.position = new Vector3((followObject.transform.position.x - _offset.x) / 2, followObject.transform.position.y + _offset.y, transform.position.z);
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
                target = new Vector3((followObject.transform.position.x + _offset.x), followObject.transform.position.y + _offset.y, transform.position.z);
            else
                target = new Vector3((followObject.transform.position.x - _offset.x), followObject.transform.position.y + _offset.y, transform.position.z);

            transform.position = Vector3.Lerp(transform.position, target, smoothing * Time.deltaTime);
        }
        else Debug.Log("Camera: Object not found.");
    }
}
