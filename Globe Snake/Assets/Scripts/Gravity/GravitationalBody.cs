using UnityEngine;

public class GravitationalBody : MonoBehaviour
{
    public GravitationalAttraction GravityAttraction;
    private Transform _transform;
    private Rigidbody _rigidbody;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
        _transform = transform; 
    }

    private void FixedUpdate()
    {
        GravityAttraction.Attract(_transform, _rigidbody);
    }

    public void SetAttraction(GravitationalAttraction attraction)
    {
        GravityAttraction = attraction;
    }
}
