using System.Collections;
using System.Collections.Generic;
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
    }
    private void OnDisable()
    {
        _movementInputAction.Disable();
    }
    
    // Update is called once per frame
    void Update()
    {
        _movementDirection = _movementInputAction.ReadValue<Vector2>();        
    }

    private void FixedUpdate()
    {
        Move();
        RotateToWalkingDirection();
    }

    private void Move()
    {
        _playerRigidbody.velocity = new Vector3(_movementDirection.x * _movementSpeed, _playerRigidbody.velocity.y, _movementDirection.y * _movementSpeed);
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
