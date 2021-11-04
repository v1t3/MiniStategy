using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Transform scaleTransform;
    [SerializeField] private Transform target;

    private Transform _cameraTransform;

    private void Start()
    {
        if (Camera.main is { })
        {
            _cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        transform.rotation = _cameraTransform.rotation;
    }

    public void Setup()
    {
        
    }

    public void SetHealth(int health, int maxHealth)
    {
        float xScale = Mathf.Clamp01((float)health / maxHealth);
        scaleTransform.localScale = new Vector3(xScale,1,1);
    }
}
