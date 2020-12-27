using System;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] protected Transform cameraTransform;
    [SerializeField] private float parallaxEffect;
    [SerializeField] private bool lockY;

    private void Awake()
    {
        if(cameraTransform != null) return;

        GameObject findObject = GameObject.Find("Main Camera");
        
        if(findObject == null) return;

        cameraTransform = findObject.transform;
    }

    void FixedUpdate()
    {
        if (lockY)
            transform.position = new Vector3(cameraTransform.position.x * parallaxEffect, transform.position.y, transform.position.z);

        else
            transform.position = new Vector3(cameraTransform.position.x * parallaxEffect, cameraTransform.position.y * parallaxEffect, transform.position.z);
    }
}
