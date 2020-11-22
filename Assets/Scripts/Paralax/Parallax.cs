using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] protected Transform cameraTransform;
    [SerializeField] private float parallaxEffect;
    [SerializeField] private bool lockY;
    void FixedUpdate()
    {
        if (lockY)
            transform.position = new Vector3(cameraTransform.position.x * parallaxEffect, transform.position.y, transform.position.z);

        else
            transform.position = new Vector3(cameraTransform.position.x * parallaxEffect, cameraTransform.position.y * parallaxEffect, transform.position.z);
    }
}
