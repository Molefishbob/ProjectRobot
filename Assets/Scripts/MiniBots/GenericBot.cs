﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericBot : CharControlBase, ITimedAction, IDamageReceiver
{
    public float _fTurnSpeed = 2;
    [Tooltip("Lifetime in seconds")]
    public float _fLifetime = 5;
    public bool _bMoving;
    public bool _bDebug;
    private Transform _tPool;
    protected OneShotTimer _lifeTimeTimer;

    protected override void Awake()
    {
        base.Awake();
        _lifeTimeTimer = GetComponent<OneShotTimer>();
        _tPool = transform.parent;
        _animator = GetComponentInChildren<Animator>();
    }

    protected override void Start()
    {
        base.Start();
        if (_bDebug)
            StartMovement();
    }

    public virtual void StartMovement()
    {
        SetControllerActive(true);
        _lifeTimeTimer.SetTimerTarget(this);
        _lifeTimeTimer.StartTimer(_fLifetime);
        _bMoving = true;
        if (_animator != null)
            _animator.SetTrigger("Run");
    }

    protected override void FixedUpdateAdditions()
    {
        if ((_controller.collisionFlags & CollisionFlags.CollidedSides) != 0)
        {
            _bMoving = false;
            if (_animator != null)
                _animator.SetTrigger("Stop");
        }
            
        RaycastHit hit;
        int tmpLayerMask = (1 << 12) | (1 << 13);
        if (_bMoving && Physics.SphereCast(transform.position + new Vector3(0,_controller.height,0) + transform.forward, 
                                0.1f, Vector3.down, out hit, _controller.height + _controller.stepOffset, tmpLayerMask))
        {
            if (hit.transform.gameObject.layer == 13)
            {
                transform.position = hit.point;
                _bMoving = false;
                if (_animator != null)
                _animator.SetTrigger("Stop");
            }
        }
        else if (_bMoving)
        {
            _bMoving = false;
            if (_animator != null)
                _animator.SetTrigger("Stop");
        }
    }

    public virtual void ResetBot()
    {
        _bMoving = false;
        _lifeTimeTimer.StopTimer();
        transform.parent = _tPool;
        transform.position = Vector3.zero;
        SetControllerActive(false);
        ResetGravity();
        gameObject.SetActive(false);
    }

    public void TimedAction()
    {
        //Used for dying
        ResetBot();
        //Play animations explode
    }

    protected void TurnTowards(GameObject target)
    {
        Vector3 v3TurnDirection = target.transform.position - transform.position;
        v3TurnDirection = v3TurnDirection.normalized;
        v3TurnDirection.y = 0;
        Quaternion qTargetRotation = Quaternion.LookRotation(v3TurnDirection);
        Quaternion qLimitedRotation = Quaternion.Lerp(transform.rotation, qTargetRotation, _fTurnSpeed * Time.deltaTime);
        transform.rotation = qLimitedRotation;
    }

    void OnEnable()
    {
        SetControllerActive(false);
        if (_bDebug)
            StartMovement();
        if (_animator != null)
            _animator.SetTrigger("Held");
    }

    protected override Vector3 InternalMovement()
    {
        if (_bMoving && _controller.isGrounded)
        {
            return transform.forward;
        }
        return Vector3.zero;
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Bot died due to damage");
        Die();
    }

    public void Die()
    {
        ResetBot();
    }
}