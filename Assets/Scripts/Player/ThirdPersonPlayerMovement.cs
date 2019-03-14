﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonPlayerMovement : CharControlBase, IDamageReceiver
{
    public string _horizontalAxis = "Horizontal";
    public string _verticalAxis = "Vertical";
    [SerializeField]
    private SingleSFXSound _walkSound;
    [SerializeField]
    private SingleSFXSound _landingSound;

    private Transform _cameraTransform;
    private bool _airBorne = false;

    protected override void Awake()
    {
        base.Awake();
        _cameraTransform = FindObjectOfType<NoZoomThirdPersonCam>().transform;
    }

    protected override void Start()
    {
        _airBorne = !IsGrounded;
    }

    protected override Vector3 InternalMovement()
    {
        // read input
        float horizontal = Input.GetAxis(_horizontalAxis);
        float vertical = Input.GetAxis(_verticalAxis);

        // create combined vector of input
        Vector3 inputDirection = new Vector3(horizontal, 0, vertical);

        // clamp magnitude
        float desiredSpeed = Mathf.Clamp(inputDirection.magnitude, 0.0f, 1.0f);

        // convert to world space relative to camera
        inputDirection = _cameraTransform.TransformDirection(inputDirection);

        // remove pitch
        inputDirection.y = 0;

        // apply magnitude
        inputDirection = inputDirection.normalized * desiredSpeed;

        if (inputDirection.magnitude > 0 && IsGrounded && !_walkSound.IsPlaying)
        {
            _walkSound.PlaySound();
        }

        return inputDirection;
    }

    protected override void FixedUpdateAdditions()
    {
        if (!_airBorne && !IsGrounded)
        {
            _airBorne = true;
        }
        else if (_airBorne && IsGrounded)
        {
            _airBorne = false;
            _landingSound.PlaySound();
        }
    }

    public void TakeDamage(int damage)
    {
        Die();
    }

    public void Die()
    {
        Debug.Log("Player died");
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
