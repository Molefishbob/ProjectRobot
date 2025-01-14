﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMinibotAnimatorMiddlehand : MonoBehaviour
{
    private BombAction _bombAction = null;
    [SerializeField]
    private RandomSFXSound _WalkSound = null;
    [SerializeField, Tooltip("Percentage")]
    private float _randomIdleChance = 10.0f;
    [SerializeField]
    private string _rngTrigger1 = "RNG1";
    [SerializeField]
    private string _rngTrigger2 = "RNG2";
    private Animator _animator;

    private BotReleaser _releaser;

    void Awake()
    {
        _bombAction = transform.parent.gameObject.GetComponent<BombAction>();
        _releaser = transform.parent.gameObject.GetComponent<BotReleaser>();
        _animator = GetComponent<Animator>();
    }

    public void Explode()
    {
        _bombAction.ExplodeBot();
    }

    public void PlayStepSound()
    {
        _WalkSound.PlaySound(true);
    }

    public void IdleCycle()
    {
        if (Random.Range(1, 101) <= _randomIdleChance)
        {
            if (Random.Range(0, 2) == 0) _animator.SetTrigger(_rngTrigger1);
            else _animator.SetTrigger(_rngTrigger2);
        }
    }

    public void HackComplete()
    {
        _releaser.ReleaseOnly();
    }
}
