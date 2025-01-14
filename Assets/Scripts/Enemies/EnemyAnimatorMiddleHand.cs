﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorMiddleHand : MonoBehaviour
{
    [SerializeField]
    private RandomSFXSound _hopsfx = null;
    private EnemyMover _frog;
    private PatrolEnemy _scorpion;
    //[SerializeField]
    //private string _deadBool = "Dead";

    private void Awake()
    {
        if (transform.parent.tag == "Frog")
        {
            _frog = GetComponentInParent<EnemyMover>();
        }
        else if (transform.parent.tag == "Scorpion")
        {
            _scorpion = GetComponentInParent<PatrolEnemy>();
        }
    }

    public void PlayHopSound()
    {
        _hopsfx.PlaySound(true);
    }

    public void AttackEnded()
    {
        if (_frog != null)
        {
            _frog._eDirect.CheckTargets();
        }
        if (_scorpion != null)
        {
            _scorpion.StopMoving = false;
        }
    }

    public void DefendEnded()
    {
        if (_scorpion != null)
        {
            _scorpion.StopMoving = false;
        }
    }

    /*public void DeathComplete()
    {
        if (_frog != null)
        {
            _frog._animator.SetBool(_deadBool, false);
            _frog.gameObject.SetActive(false);
        }
        if (_scorpion != null)
        {
            _scorpion._animator.SetBool(_deadBool, false);
            _scorpion.gameObject.SetActive(false);
        }
    }*/

    public void AlertEnded()
    {
        if (_frog._eDirect._aggroTargets.Count > 0)
        {
            _frog.StopMoving = false;
            _frog._animator.SetBool("Jump", true);
        }
        else
        {
            _frog._animator.SetBool("Jump", false);
            _frog._eDirect.Idle();
        }
    }
}
