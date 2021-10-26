using System;
using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    [SerializeField] private GameObject selectionIndicator;

    private void Start()
    {
        selectionIndicator.SetActive(false);
    }

    public virtual void OnHover()
    {
        transform.localScale = Vector3.one * 1.1f;
    }

    public virtual void OnUnhover()
    {
        transform.localScale = Vector3.one;
    }

    public virtual void Select()
    {
        selectionIndicator.SetActive(true);
    }

    public virtual void Unselect()
    {
        selectionIndicator.SetActive(false);
    }
}
