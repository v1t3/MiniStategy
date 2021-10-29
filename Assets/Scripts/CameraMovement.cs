using System;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera raycastCamera;
    
    private Vector3 _startPoint;
    private Vector3 _cameraStartPosition;
    private Plane _plane;

    private void Start()
    {
        _plane = new Plane(Vector3.up, Vector3.zero);
    }

    private void Update()
    {
        Ray ray = raycastCamera.ScreenPointToRay(Input.mousePosition);
        float distance;
        _plane.Raycast(ray, out distance);
        Vector3 point = ray.GetPoint(distance);

        if (Input.GetMouseButtonDown(2))
        {
            _startPoint = point;
            _cameraStartPosition = mainCamera.transform.position;
        }

        if (Input.GetMouseButton(2))
        {
            Vector3 offset = point - _startPoint;
            Debug.Log("offset " + offset);
            mainCamera.transform.position = _cameraStartPosition - offset;
        }
        
        mainCamera.transform.Translate(0,0, Input.mouseScrollDelta.y);
        raycastCamera.transform.Translate(0,0, Input.mouseScrollDelta.y);
    }
}