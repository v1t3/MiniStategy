using System;
using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    [SerializeField] private GameObject selectionIndicator;

    public virtual void Start()
    {
        if (selectionIndicator)
        {
            selectionIndicator.SetActive(false);
        }
    }

    public virtual void Select()
    {
        if (selectionIndicator)
        {
            selectionIndicator.SetActive(true);
        }
    }

    public virtual void Unselect()
    {
        if (selectionIndicator)
        {
            selectionIndicator.SetActive(false);
        }
    }

    public virtual void OnHover()
    {
    }

    public virtual void OnUnhover()
    {
    }

    public virtual void OnClickOnGround(Vector3 point)
    {
    }
}