using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

	private void Update()
	{
        var eventSystem = GameManager.Instance.EventSystem;

		if (Input.GetMouseButtonDown(0))
		{
			var pointerEventData = new PointerEventData(eventSystem);
			pointerEventData.position = Input.mousePosition;

			var uiRaycastResults = new List<RaycastResult>();
			eventSystem.RaycastAll(pointerEventData, uiRaycastResults);

			if (uiRaycastResults.Count > 0)
			{
				// We assume only one Result is valid (the 1st one)
				var raycastResult = uiRaycastResults[0];
                var raycastGameObject = raycastResult.gameObject;

				if (raycastGameObject.name.Equals("Image"))
				{
                    var imageComponent = raycastGameObject.GetComponent<Image>();
                    // The associated SpawnPoint of the Image/Canvas (parent.parent)
                    var spawnPoint = raycastGameObject.transform.parent.parent;

					switch (imageComponent.sprite.name)
                    {
                    case "RemoveItem":
                        ItemSpawner.Instance.RemoveItem(spawnPoint);
                        imageComponent.sprite = ItemSpawner.Instance.AddItemImage;

					    break;

					case "AddItem":
						ItemSpawner.Instance.AddItem(spawnPoint);

						break;

                    default:
                        Debug.Log("PlayerMovement: UI Raycast; Name of Sprite of ImageComponent is not recognized!");

                        break;
					}
				}
			}
		}
	}
}
