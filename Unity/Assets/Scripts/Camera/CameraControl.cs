using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
	[SerializeField] private GameObject _player;
	//[SerializeField] private Vector3 _offset = new Vector3(0, 5, -5);

	public float OffsetY = 3.0f;
	public float DistanceToPlayer = 2.0f;

	private Vector2 m_CameraAngles = new();
	public float FocusLength = 4.0f;

	private void Start()
	{
		m_CameraAngles.x = 180.0f;
	}

	private void Update()
	{
		if (PlayerHUDController.Instance.IsPaused())
		{
			return;
		}

		//transform.position = _player.transform.position + _offset;

		if (Input.GetMouseButton(1))
		{
			m_CameraAngles.x += Input.GetAxis("Mouse X");
			if (m_CameraAngles.x >= 360.0f)
			{
				m_CameraAngles.x -= 360.0f;
			}
			else if (m_CameraAngles.x <= -360.0f)
			{
				m_CameraAngles.x += 360.0f;
			}

			m_CameraAngles.y += Input.GetAxis("Mouse Y");
			m_CameraAngles.y = Mathf.Clamp(m_CameraAngles.y, -10.0f, 10.0f);
		}

		var playerPos = _player.transform.position;
		var ourNewPos = playerPos;
		ourNewPos.y += OffsetY;

		// Always normalized
		var cameraCircleTranslation = new Vector3(Mathf.Sin(m_CameraAngles.x * Mathf.Deg2Rad), 0.0f,
											Mathf.Cos(m_CameraAngles.x * Mathf.Deg2Rad));
		cameraCircleTranslation *= DistanceToPlayer;
		ourNewPos += cameraCircleTranslation;

		// Fokussieren -> Mausradbutton gedr¸ckt halten -> Camera schieﬂt nach vorne Richtung Cursor
		if (Input.GetMouseButton(2))
		{
			var cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			var camDirWithoutY = new Vector3(transform.forward.x, 0.0f, transform.forward.z).normalized;
			
			// Kosinus des Winkels zwischen Camera.forward (nur geradeaus; y = 0.0) und des cursorRays
			var angleBetweenCursorRayAndCamDir = Vector3.Dot(cursorRay.direction, camDirWithoutY);
			// FocusLength muss angepasst werden, wegen Pythagoras und so
			var transformedFocusLength = FocusLength / angleBetweenCursorRayAndCamDir;

			transform.position = ourNewPos + cursorRay.direction*transformedFocusLength;
		}
		else
		{
			transform.position = ourNewPos;
			transform.LookAt(new Vector3(_player.transform.position.x, transform.position.y + m_CameraAngles.y/10.0f, _player.transform.position.z));

			var curCamRotation = transform.rotation.eulerAngles;
			_player.transform.rotation = Quaternion.Euler(0.0f, curCamRotation.y, 0.0f);
			transform.localRotation = Quaternion.Euler(curCamRotation.x, 0.0f, 0.0f);
		}
	}
}
