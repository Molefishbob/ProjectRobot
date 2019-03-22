using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void ValueChangedInt(int amount);
public delegate void ValueChangedFloat(float amount);
public delegate void ValueChangedBool(bool value);

public enum MiniBotAbility
{
    Hack,
    Bomb,
    Tramp
}

public class GameManager : Singleton<GameManager>
{
    // (Optional) Prevent non-singleton constructor use.
    protected GameManager() { }
    public event ValueChangedInt OnBotAmountChanged;
    public event ValueChangedInt OnMaximumBotAmountChanged;
    public event ValueChangedBool OnGamePauseChanged;

    /// <summary>
    /// Is the game currently paused?
    /// </summary>
    public bool GamePaused { get; private set; }

    // TODO: make bot actions check this collection
    public HashSet<MiniBotAbility> UsableMiniBotAbilities;

    public int CurrentBotAmount
    {
        get
        {
            return _currentBotAmount;
        }
        set
        {
            _currentBotAmount = value;

            if (OnBotAmountChanged != null)
            {
                OnBotAmountChanged(_currentBotAmount);
            }
        }
    }

    private int _currentBotAmount;

    public int MaximumBotAmount
    {
        get
        {
            return _maximumBotAmount;
        }
        set
        {
            _maximumBotAmount = value;

            if (OnMaximumBotAmountChanged != null)
            {
                OnMaximumBotAmountChanged(_maximumBotAmount);
            }
        }
    }

    private int _maximumBotAmount;

    /// <summary>
    /// Reference of the player.
    /// </summary>
    public PlayerMovement Player;

    private LevelManager _levelManager;

    /// <summary>
    /// Reference of the LevelManager.
    /// </summary>
    public LevelManager LevelManager
    {
        get
        {
            if (_levelManager == null)
            {
                Debug.LogError("LEVELMANAGER MISSING!!!!");
            }
            return _levelManager;
        }
        set
        {
            _levelManager = value;
        }
    }

    public ControlledBotPool BotPool;

    public PatrolEnemyPool PatrolEnemyPool;
    public FrogEnemyPool FrogEnemyPool;

    public ThirdPersonCamera Camera;

    public int CurrentLevel { get; private set; } = 1;

    private float _timeScaleBeforePause = 1.0f;

    /// <summary>
    /// Pauses all in-game objects and sets timescale to 0.
    /// </summary>
    public void PauseGame()
    {
        if (!GamePaused)
        {
            GamePaused = true;
            _timeScaleBeforePause = Time.timeScale;
            Time.timeScale = 0;
            if (OnGamePauseChanged != null)
            {
                OnGamePauseChanged(true);
            }
        }
        else
        {
            Debug.LogWarning("Game is already paused.");
        }
    }

    /// <summary>
    /// Unpauses all in-game objects and sets timescale back to what it was before pausing.
    /// </summary>
    public void UnPauseGame()
    {
        if (GamePaused)
        {
            GamePaused = false;
            Time.timeScale = _timeScaleBeforePause;
            if (OnGamePauseChanged != null)
            {
                OnGamePauseChanged(false);
            }
        }
        else
        {
            Debug.LogWarning("Game is already unpaused.");
        }
    }

    public void QuitGame()
    {
        PrefsManager.Instance.Save();
        Application.Quit();
    }

    public void ChangeScene(int id)
    {
        SceneManager.LoadScene(id);
    }

    public void ChangeToMainMenu()
    {
        ChangeScene(0);
    }

    public void NextLevel()
    {
        CurrentLevel++;
        ChangeScene(CurrentLevel);
    }

    public void StartNewGame()
    {
        CurrentLevel = 1;
        ChangeScene(CurrentLevel);
    }

    public void ReloadScene()
    {
        ChangeScene(CurrentLevel);
    }

    public void UndoDontDestroy(GameObject go)
    {
        SceneManager.MoveGameObjectToScene(go, SceneManager.GetActiveScene());
    }
}