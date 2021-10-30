using System.Collections.Generic;
using Units;
using UnityEngine;
using UnityEngine.UI;

public enum BuildState
{
    Installing,
    Other
}

public enum SelectionState
{
    UnitsSelected,
    FrameActive,
    Other
}

public class Management : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    private SelectableObject _hovered;

    [SerializeField] private List<SelectableObject> listOfSelected;

    [SerializeField] private Image frameImage;
    private Vector2 _frameStart;
    private Vector2 _frameEnd;
    [SerializeField] private float cursorDeltaValue = 10;

    public BuildState currentBuildState = BuildState.Other;
    public SelectionState currentSelectionState = SelectionState.Other;

    private void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red);

        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider && hit.collider.GetComponent<SelectableCollider>())
            {
                var hitSelectable = hit.collider.GetComponent<SelectableCollider>().selectableObject;
                HoverCurrent(hitSelectable);
            }
            else
            {
                UnhoverCurrent();
            }
        }
        else
        {
            UnhoverCurrent();
        }

        if (Input.GetMouseButtonUp(0) && _hovered)
        {
            if (!Input.GetKey(KeyCode.LeftControl))
            {
                UnselectAll();
            }

            currentSelectionState = SelectionState.UnitsSelected;

            Select(_hovered);
        }

        if (Input.GetMouseButtonUp(0) && currentSelectionState == SelectionState.UnitsSelected)
        {
            if (hit.collider && hit.collider.CompareTag("Ground"))
            {
                foreach (var item in listOfSelected)
                {
                    item.OnClickOnGround(hit.point);
                }
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            UnselectAll();
        }

        // выделение рамкой
        if (Input.GetMouseButtonDown(0))
        {
            _frameStart = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            _frameEnd = Input.mousePosition;

            Vector2 minPoint = Vector2.Min(_frameStart, _frameEnd);
            Vector2 maxPoint = Vector2.Max(_frameStart, _frameEnd);
            Vector2 delta = maxPoint - minPoint;

            if (delta.magnitude > cursorDeltaValue)
            {
                frameImage.enabled = true;
                frameImage.rectTransform.anchoredPosition = minPoint;
                frameImage.rectTransform.sizeDelta = delta;

                Rect selectRect = new Rect(minPoint, delta);
                Unit[] allUnits = FindObjectsOfType<Unit>();

                UnselectAll();

                foreach (var unit in allUnits)
                {
                    Vector2 screenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);

                    if (selectRect.Contains(screenPosition))
                    {
                        Select(unit);
                    }
                }

                currentSelectionState = SelectionState.FrameActive;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            frameImage.enabled = false;

            currentSelectionState = listOfSelected.Count > 0 ? SelectionState.UnitsSelected : SelectionState.Other;
        }
    }

    private void Select(SelectableObject selectableObject)
    {
        if (!listOfSelected.Contains(selectableObject))
        {
            listOfSelected.Add(selectableObject);
            selectableObject.Select();
        }
    }

    public void UnselectAll()
    {
        foreach (var item in listOfSelected)
        {
            item.Unselect();
        }

        listOfSelected.Clear();
        
        currentSelectionState = SelectionState.Other;
    }

    private void HoverCurrent(SelectableObject selectableObject)
    {
        if (currentBuildState != BuildState.Other) return;
        
        if (!_hovered)
        {
            _hovered = selectableObject;
            _hovered.OnHover();
        }
        else if (_hovered != selectableObject)
        {
            _hovered.OnUnhover();
            _hovered = selectableObject;
            _hovered.OnHover();
        }
    }

    private void UnhoverCurrent()
    {
        if (currentBuildState != BuildState.Other) return;

        if (_hovered)
        {
            _hovered.OnUnhover();
            _hovered = null;
        }
    }
}