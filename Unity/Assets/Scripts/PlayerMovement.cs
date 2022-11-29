using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody _playerRigidbody;
    [SerializeField] private float _movementSpeed = 3.0f;
    private Vector2 _movementDirection = Vector2.zero;
    private PlayerInputActions _playerControls;
    private InputAction _movementInputAction;

    private void Awake()
    {
        _playerControls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _movementInputAction = _playerControls.Player.Move;
        _movementInputAction.Enable();
        _movementInputAction.performed += MovePerformed;
        _movementInputAction.canceled += MovePerformed;
    }

    private void MovePerformed(InputAction.CallbackContext context)
    {

        _movementDirection = _movementInputAction.ReadValue<Vector2>();        
        RotateToWalkingDirection();
    }

    private void OnDisable()
    {
        _movementInputAction.Disable();
    }

    private void FixedUpdate()
    {
        _playerRigidbody.velocity = new Vector3(_movementDirection.x * _movementSpeed, _playerRigidbody.velocity.y, _movementDirection.y * _movementSpeed);
        //RotateToWalkingDirection();
    }

    private void RotateToWalkingDirection()
    {
        //float angle = Vector3.Angle(movementDirection, transform.forward);
        //playerRigidbody.rotation = Quaternion.Euler(0, angle, 0);
        //playerRigidbody.MoveRotation(Quaternion.Euler(0, angle, 0));
        if (_movementDirection != Vector2.zero) 
        {
            transform.forward = new Vector3(_movementDirection.x, 0, _movementDirection.y);
        }
    }
}
