using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float m_movementSpeed = 3.0f;
    private Vector2 m_movementDirection = Vector2.zero;
    private Rigidbody m_playerRigidbody;
    private PlayerInputActions m_playerControls;
    private InputAction m_movementInputAction;

    private void Awake()
    {
        m_playerRigidbody = GetComponent<Rigidbody>();
        m_playerControls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        m_movementInputAction = m_playerControls.Player.Move;
        m_movementInputAction.Enable();
    }

    private void OnDisable()
    {
        m_movementInputAction.Disable();
    }

    private void Update()
    {
        m_movementDirection = m_movementInputAction.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (PlayerHUDController.Instance.IsPaused())
            return;

        //_playerRigidbody.velocity = new Vector3(_movementDirection.x * _movementSpeed, _playerRigidbody.velocity.y, _movementDirection.y * _movementSpeed);
        m_playerRigidbody.velocity = transform.right * m_movementSpeed * m_movementDirection.x +
                                    Vector3.up * m_playerRigidbody.velocity.y +
                                    transform.forward * m_movementSpeed * m_movementDirection.y;
    }
}
