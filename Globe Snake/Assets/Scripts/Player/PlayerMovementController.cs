using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 10f;
    [SerializeField] private float _turnSpeed = 180f;
    [SerializeField] private Rigidbody _rigidBody;
    public InputActionAsset playerControls;

    private float _turnDirection;
    private Vector2 _movementVector;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    public void Steer(InputAction.CallbackContext context)
    {
        //if (context.started)
        //{
            //Debug.Log("Steering");
            _turnDirection = context.ReadValue<float>();
        //}
    }

    private void FixedUpdate()
    {
        _rigidBody.velocity = transform.forward * _movementSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up * _turnDirection * _turnSpeed * Time.deltaTime);
    }

    public void ChangeMovementSpeed(float speedPoints)
    {
        _movementSpeed += speedPoints;
        if (_movementSpeed < 50)
            _movementSpeed = 50;
    }

    public float GetMovementSpeed()
    {
        return _movementSpeed;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }
}
