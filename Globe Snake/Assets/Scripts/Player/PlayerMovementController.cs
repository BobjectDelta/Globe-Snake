using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 100f;
    [SerializeField] private float _turnSpeed = 180f;
    [SerializeField] private Rigidbody _rigidBody;
    public InputActionAsset playerControls;

    private float _turnDirection;
    private float _initialMovementSpeed;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _initialMovementSpeed = _movementSpeed;
    }

    public void Steer(InputAction.CallbackContext context)
    {
        //if (context.started)
        //{
            _turnDirection = context.ReadValue<float>();
            Debug.Log("Steering: " + _turnDirection);

        //}
    }

    private void FixedUpdate()
    {
        _rigidBody.velocity = transform.forward * _movementSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up * _turnDirection * _turnSpeed * Time.deltaTime);
    }

    public void ChangeMovementSpeed(float speedPoints)
    {
        bool isMovementSpeedValid = _movementSpeed + speedPoints >= _initialMovementSpeed / 4 &&
            _movementSpeed + speedPoints <= _initialMovementSpeed * 4;
        if (isMovementSpeedValid)
            _movementSpeed += speedPoints;
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
