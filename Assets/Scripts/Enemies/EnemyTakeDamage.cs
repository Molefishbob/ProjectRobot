﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTakeDamage : MonoBehaviour, IDamageReceiver
{
    [SerializeField]
    private string _deadBool = "Dead";
    private EnemyMover _frog;

    private void Awake()
    {
        _frog = GetComponent<EnemyMover>();
    }

    public void TakeDamage(int damage)
    {
        Die();
    }

    public void Die()
    {
        _frog.SetControllerActive(false);
        _frog._attack.gameObject.SetActive(false);
        _frog._animator.SetBool(_deadBool, true);
    }
}
