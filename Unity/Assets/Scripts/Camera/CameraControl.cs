using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private Vector3 _offset = new Vector3(0, 9, -9);

    private void Update()
    {
        transform.position = _player.transform.position + _offset;
    }
}
