using System.Collections.Generic;
using UnityEngine;

public class Management : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private SelectableObject hovered;
    
    [SerializeField] private List<SelectableObject> listOfSelected;

    private void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red);

        if (Physics.Raycast(ray, out var hit))
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

        if (Input.GetMouseButtonUp(0))
        {
            if (hovered)
            {
                if (!Input.GetKey(KeyCode.LeftControl))
                {
                    UnselectAll();
                }

                Select(hovered);
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            UnselectAll();
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

    private void UnselectAll()
    {
        foreach (var item in listOfSelected)
        {
            item.Unselect();
        }

        listOfSelected.Clear();
    }

    private void HoverCurrent(SelectableObject selectableObject)
    {
        if (!hovered)
        {
            hovered = selectableObject;
            hovered.OnHover();
        }
        else if (hovered != selectableObject)
        {
            hovered.OnUnhover();
            hovered = selectableObject;
            hovered.OnHover();
        }
    }

    private void UnhoverCurrent()
    {
        if (hovered)
        {
            hovered.OnUnhover();
            hovered = null;
        }
    }
}