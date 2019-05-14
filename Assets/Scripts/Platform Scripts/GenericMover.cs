﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericMover : MonoBehaviour, IButtonInteraction
{

    [Tooltip("The amount of time it takes to go the whole length")]
    public float _duration;
    [Tooltip("The duration after which the symbol goes off")]
    public float _delayDuration = 0.4f;
    protected float _eventTime;
    public List<Transform> _transform;
    protected float _fracTime;
    protected int _amountOfTransforms;
    protected int _currentObjectNum = 0;
    protected int _nextObjectNum = 1;
    protected float _length;
    protected float _ogStartTime;
    protected int _trackRecord = 0;
    protected PhysicsRepeatingTimer _timer;
    protected ScaledOneShotTimer _delayTimer;
    [SerializeField]
    protected bool _activated = true;
    [SerializeField]
    protected GameObject _symbol = null;
    [SerializeField]
    protected SingleSFXSound _moveSound = null;
    [HideInInspector]
    public Vector3 CurrentMove { get; protected set; } = Vector3.zero;

    protected virtual void Awake()
    {
        _timer = gameObject.AddComponent<PhysicsRepeatingTimer>();
        _transform = new List<Transform>(transform.parent.childCount);
        _delayTimer = gameObject.AddComponent<ScaledOneShotTimer>();

        foreach (Transform child in transform.parent)
        {
            if (child != transform)
            {
                _transform.Add(child);
            }
        }

        _amountOfTransforms = _transform.Count - 1;
    }

    protected virtual void Start()
    {
        _timer.OnTimerCompleted += TimedAction;
        _delayTimer.OnTimerCompleted += SymbolDown;
        transform.position = _transform[0].position;
        if (_activated)
        {
             if (_symbol != null)
                _symbol.SetActive(false);
            Init();
        }
    }

    private void OnDestroy()
    {
        if (_timer != null)
            _timer.OnTimerCompleted -= TimedAction;
        if (_delayTimer != null)
            _delayTimer.OnTimerCompleted -= SymbolDown;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.GamePaused) return;

        CurrentMove = InternalMove() - transform.position;
        transform.position += CurrentMove;
    }

    protected abstract Vector3 InternalMove();

    /// <summary>
    /// Initializes the script.
    ///
    /// Gets all the checkpoints and counts the complete length of the trip.
    /// The length is later used to calculate the speed between objects.
    /// </summary>
    public virtual void Init()
    {
        _moveSound.PlaySound();
        _length = 0;

        _timer.StartTimer(_duration);

        for (int a = 0; a < _transform.Count - 1; a++)
        {
            _length += (_transform[a].position - _transform[a + 1].position).magnitude;
        }
    }
    /// <summary>
    /// Called when the timer is completed.
    ///
    /// Defines what happens when the timer has completed.
    /// </summary>
    protected virtual void TimedAction() { }

    /// <summary>
    /// Turns off the symbol that indicates the console connection
    /// </summary>
    protected virtual void SymbolDown()
    {
        _activated = true;
        Init();
        if (_symbol != null)
        {
            _symbol.SetActive(false);
        }
    }
    /// <summary>
    /// Defines what happens when a connected console is hacked
    /// </summary>
    public virtual bool ButtonDown(float actionDelay)
    {
        if (!_activated)
        {
            _delayTimer.StartTimer(actionDelay + _delayDuration);
            return true;
        }
        return false;
    }

    public bool ButtonUp()
    {
        return false;
    }

    /// <summary>
    /// Draws lines between the checkpoints that the platform moves through.
    /// </summary>
    protected virtual void OnDrawGizmosSelected()
    {
        if (transform.parent == null) return;

        _transform = new List<Transform>(transform.parent.childCount);

        foreach (Transform child in transform.parent)
        {
            if (child != transform)
            {
                _transform.Add(child);
            }
        }
        for (int a = 0; a < _transform.Count; a++)
        {
            if (a + 1 != _transform.Count)
            {
                Gizmos.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
                Gizmos.DrawLine(_transform[a].position, _transform[a + 1].position);
            }
        }
        Gizmos.color = new Color(0, 0, 0, 1);
        Gizmos.DrawWireCube(_transform[0].position, new Vector3(3, 0.5f, 2));
        Gizmos.DrawWireCube(_transform[_transform.Count - 1].position, new Vector3(3, 0.5f, 2)); ;
    }
}
