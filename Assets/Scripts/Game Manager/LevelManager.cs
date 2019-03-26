﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private int _currentCheckPoint = 0;
    private CheckPointPole[] _allLevelCheckPoints;
    public MainCharMovement _playerPrefab;
    public ThirdPersonCamera _cameraPrefab;
    /// <summary>
    /// The pool prefab
    /// </summary>
    public ControlledBotPool _botPoolPrefab;
    /// <summary>
    /// The pool prefab
    /// </summary>
    public FrogEnemyPool _frogPoolPrefab;
    /// <summary>
    /// The pool prefab
    /// </summary>
    public PatrolEnemyPool _patrolEnemyPoolPrefab;

    public bool _levelNeedsFrogEnemies;
    public bool _levelNeedsPatrolEnemies;

    void Awake()
    {
        PrefsManager.Instance.Level = SceneManager.GetActiveScene().buildIndex;
        PrefsManager.Instance.Save();

        GameManager.Instance.LevelManager = this;

        if (GameManager.Instance.BotPool == null)
        {
            GameManager.Instance.BotPool = Instantiate(_botPoolPrefab);
        }

        if (_levelNeedsFrogEnemies && GameManager.Instance.FrogEnemyPool == null)
        {
            GameManager.Instance.FrogEnemyPool = Instantiate(_frogPoolPrefab);
        }
        if (_levelNeedsPatrolEnemies && GameManager.Instance.PatrolEnemyPool == null)
        {
            GameManager.Instance.PatrolEnemyPool = Instantiate(_patrolEnemyPoolPrefab);
        }

        _allLevelCheckPoints = FindObjectsOfType<CheckPointPole>();

        if (GameManager.Instance.Player == null)
        {
            PlayerMovement player = FindObjectOfType<PlayerMovement>();
            if (player == null)
            {
                player = Instantiate(_playerPrefab);
            }
            DontDestroyOnLoad(player);
            GameManager.Instance.Player = player;
        }

        if (GameManager.Instance.Camera == null)
        {
            ThirdPersonCamera camera = FindObjectOfType<ThirdPersonCamera>();
            if (camera == null)
            {
                camera = Instantiate(_cameraPrefab);
            }
            DontDestroyOnLoad(camera);
            GameManager.Instance.Camera = camera;
        }
    }

    private void Start()
    {
        SortCheckpoints();

        if (_allLevelCheckPoints != null && _allLevelCheckPoints.Length > 0)
        {
            SetCheckpoint(0);
        }
        else
        {
            Debug.LogError("There are no checkpoints in this level.");
        }

        GameManager.Instance.Player.transform.position = GetSpawnLocation();
        GameManager.Instance.Player.transform.rotation = GetSpawnRotation();
        GameManager.Instance.Player.SetControllerActive(true);
        GameManager.Instance.Camera.GetInstantNewTarget(GameManager.Instance.Player.transform);

        GameManager.Instance.ActivateGame(true);
    }

    /// <summary>
    /// Gives the spawn location from current checkpoint.
    /// </summary>
    /// <returns>Spawn location</returns>
    private Vector3 GetSpawnLocation()
    {
        if (_allLevelCheckPoints == null || _allLevelCheckPoints[_currentCheckPoint] == null)
            return Vector3.zero;
        else
            return _allLevelCheckPoints[_currentCheckPoint].SpawnPoint.position;
    }

    private Quaternion GetSpawnRotation()
    {
        if (_allLevelCheckPoints == null || _allLevelCheckPoints[_currentCheckPoint] == null)
            return Quaternion.identity;
        else
            return _allLevelCheckPoints[_currentCheckPoint].SpawnPoint.rotation;
    }

    public void SetCheckpoint(int id)
    {
        if (id > _currentCheckPoint)
        {
            _currentCheckPoint = id;
            PrefsManager.Instance.CheckPoint = _currentCheckPoint;
            PrefsManager.Instance.Save();
        }
    }

    private void SortCheckpoints()
    {
        bool sorted = false;
        while (!sorted)
        {
            bool swapped = false;
            for (int i = 0; i < _allLevelCheckPoints.Length - 1; i++)
            {
                if (_allLevelCheckPoints[i].id > _allLevelCheckPoints[i + 1].id)
                {
                    CheckPointPole tmp = _allLevelCheckPoints[i];
                    _allLevelCheckPoints[i] = _allLevelCheckPoints[i + 1];
                    _allLevelCheckPoints[i + 1] = tmp;
                    swapped = true;
                }
                else if (_allLevelCheckPoints[i].id == _allLevelCheckPoints[i + 1].id)
                {
                    Debug.LogError("THERE ARE 2 OR MORE CHECKPOINTS WITH SAME ID!! FIX!!");
                    swapped = false;
                    break;
                }
            }
            sorted = !swapped;
        }
    }

    public void ResetLevel()
    {
        GameManager.Instance.BotPool.ResetPool();
        GameManager.Instance.FrogEnemyPool?.ResetPool();
        GameManager.Instance.PatrolEnemyPool?.ResetPool();
        DontDestroyOnLoad(gameObject);
        GameManager.Instance.ReloadScene();
        GameManager.Instance.UndoDontDestroy(gameObject);
        Awake();
        Start();
    }
}
