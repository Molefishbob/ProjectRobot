﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoZoomThirdPersonCam : MonoBehaviour, IPauseable
{
    public LayerMask _groundLayer;
    private Transform _lookAt;
    public float _distance = 5.0f;
    public string _cameraXAxis = "Camera X";
    public string _cameraYAxis = "Camera Y";
    private float _yaw = 0.0f;
    private float _pitch = 0.0f;
    public float _horizontalSensitivity = 1.0f;
    public float _verticalSensitivity = 1.0f;
    private bool _paused;
    private float _newDistance;

    public void Pause()
    {
        _paused = true;
    }

    public void UnPause()
    {
        _paused = false;
    }

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _newDistance = _distance;
        _lookAt = gameObject.transform.parent;
    }

    private void Start()
    {
        _paused = GameManager.Instance.GamePaused;
        GameManager.Instance.AddPauseable(this);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RemovePauseable(this);
        }
    }

    private void Update()
    {
        _yaw += _horizontalSensitivity * Input.GetAxis(_cameraXAxis);
        _pitch -= _verticalSensitivity * Input.GetAxis(_cameraYAxis);

        if (_pitch > 85)
        {
            _pitch = 85;
        }
        else if (_pitch < -70)
        {
            _pitch = -70;
        }

        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0);

        _newDistance = CheckCollision(_newDistance);

        Vector3 dir = new Vector3(0, 0, -_newDistance);

        transform.position = _lookAt.position + rotation * dir;
        transform.LookAt(_lookAt.position);
    }

    private float CheckCollision(float tDistance)
    {

        RaycastHit hit;

        if (Physics.SphereCast(_lookAt.position, 0.5f, transform.TransformDirection(Vector3.back), out hit, _distance, _groundLayer))
        {
            Debug.DrawLine(_lookAt.position, hit.point, Color.red, 1.0f, false);
            float newDistance = Vector3.Distance(hit.point, _lookAt.position);
            tDistance = newDistance;
        }
        else
        {
            tDistance = _distance;
        }

        return tDistance;
    }
}