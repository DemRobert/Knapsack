using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 3.0f;
    private Vector2 _movementDirection = Vector2.zero;
    private Rigidbody _playerRigidbody;
    private PlayerInputActions _playerControls;
    private InputAction _movementInputAction;

    private void Awake()
    {
        _playerRigidbody = GetComponent<Rigidbody>();
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

    private void Update()
    {
        _movementDirection = _movementInputAction.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (PlayerHUDController.Instance.IsPaused())
            return;

        //_playerRigidbody.velocity = new Vector3(_movementDirection.x * _movementSpeed, _playerRigidbody.velocity.y, _movementDirection.y * _movementSpeed);
        _playerRigidbody.velocity = transform.right * _movementSpeed * _movementDirection.x +
                                    Vector3.up * _playerRigidbody.velocity.y +
                                    transform.forward * _movementSpeed * _movementDirection.y;
    }
}
