﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackAction : BotActionBase
{
    [SerializeField]
    private string _sHackButton = "Hack Action";
    public LayerMask HackLayer { get { return _lHackLayer; } }
    [SerializeField]
    private LayerMask _lHackLayer = 1 << 18;
    private List<GenericHackable> _hackTargets = null;
    public bool Hacking { get { return _bHacking; } }
    private bool _bHacking = false;
    private BotReleaser _releaser = null;
    private NewMinibotAnimatorMiddlehand _animMiddlehand;
    [SerializeField]
    private SingleSFXSound _hackSound = null;
    [SerializeField]
    private GameObject[] _particleEffects = null;

    protected override void Awake()
    {
        base.Awake();
        _releaser = GetComponent<BotReleaser>();
        _animMiddlehand = GetComponentInChildren<NewMinibotAnimatorMiddlehand>(true);
        _hackTargets = new List<GenericHackable>(4);
    }

    private void Update()
    {
        if (GameManager.Instance.GamePaused)
        {
            _bPaused = true;
            return;
        }
        if (_bPaused)
        {
            _bPaused = false;
            return;
        }

        if (_bHacking)
        {
            Vector3 targetDirection = _hackTargets[0].transform.position - transform.position;
            targetDirection.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _selfMover.TurningSpeed * Time.deltaTime);
            if (_hackTargets[0].CurrentStatus == GenericHackable.Status.Hacked)
            {
                _hackSound.StopSound();
                _selfMover._animator.SetBool("Hack", false);
                _hackTargets.Clear();
                _bHacking = false;
                ToggleParticleEffectsOff();
                _releaser.DeadButNotDead();
            }
            return;
        }

        if (!_selfMover.IsGrounded || !_bCanAct) return;

        if (Input.GetButtonDown(_sHackButton))
        {
            if (_hackTargets != null && _hackTargets.Count > 0)
            {
                if (_hackTargets[0].CurrentStatus == GenericHackable.Status.NotHacked)
                {
                    _hackSound.PlaySound(false);
                    _selfMover._animator.SetBool("Hack", true);
                    _bHacking = true;
                    ToggleParticleEffectsOn();
                    _hackTargets[0].TimeToStart();
                    _releaser.DisableActing();
                    _selfMover.ControlsDisabled = true;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GenericHackable ghOther = other.GetComponent<GenericHackable>();
        if (ghOther == null) return;
        _hackTargets.Add(ghOther);
    }

    private void OnTriggerExit(Collider other)
    {
        if (_bHacking) return;
        GenericHackable ghOther = other.GetComponent<GenericHackable>();
        if (ghOther == null) return;
        ghOther.TimeToLeave();
        _hackTargets.Remove(ghOther);
        ToggleParticleEffectsOff();
    }

    public override void DisableAction()
    {
        foreach (GenericHackable item in _hackTargets)
        {
            item.TimeToLeave();
        }
        _hackTargets.Clear();
        _bHacking = false;
        ToggleParticleEffectsOff();
        _selfMover._animator.SetBool("Hack", false);
    }

    public void ToggleParticleEffectsOn()
    {
        foreach (GameObject effect in _particleEffects)
        {
            effect.SetActive(true);
        }
    }
    public void ToggleParticleEffectsOff()
    {
        foreach (GameObject effect in _particleEffects)
        {
            effect.SetActive(false);
        }
    }
}
