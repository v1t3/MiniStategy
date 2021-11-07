using System;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera raycastCamera;
    private Transform _mainCameraTransform;
    private Transform _raycastCameraTransform;

    [SerializeField] private float minHeight;
    [SerializeField] private float maxHeight;

    private Vector3 _startPoint;
    private Vector3 _cameraStartPosition;
    private Plane _plane;

    private void Start()
    {
        _plane = new Plane(Vector3.up, Vector3.zero);
        _mainCameraTransform = mainCamera.transform;
        _raycastCameraTransform = raycastCamera.transform;
    }

    private void Update()
    {
        Ray ray = raycastCamera.ScreenPointToRay(Input.mousePosition);
        _plane.Raycast(ray, out float distance);
        Vector3 point = ray.GetPoint(distance);

        #region Перемещение камеры

        if (Input.GetMouseButtonDown(2))
        {
            _startPoint = point;
            _cameraStartPosition = _mainCameraTransform.position;
        }

        if (Input.GetMouseButton(2))
        {
            Vector3 offset = point - _startPoint;
            Debug.Log("offset " + offset);
            _mainCameraTransform.position = _cameraStartPosition - offset;
        }

        #endregion Перемещение камеры

        //Приблежение/отдаление камеры
        if (
            _mainCameraTransform.position.y > minHeight && Input.mouseScrollDelta.y > 0 ||
            _mainCameraTransform.position.y < maxHeight && Input.mouseScrollDelta.y < 0
        )
        {
            _mainCameraTransform.Translate(0, 0, Input.mouseScrollDelta.y);
            _raycastCameraTransform.Translate(0, 0, Input.mouseScrollDelta.y);
        }
    }
}