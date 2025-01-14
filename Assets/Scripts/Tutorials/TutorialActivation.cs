using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialActivation : MonoBehaviour
{
    [SerializeField, Tooltip("0 means not hidden by time")]
    private float _hideTime = 0.0f;
    [SerializeField, Tooltip("0 means never comes back")]
    private float _reshowTime = 0.0f;
    [SerializeField, Tooltip("Time delay until camera or movement will hide tutorial")]
    private float _delayTime = 2.0f;
    [SerializeField]
    private bool _showWhileInArea = false;
    [SerializeField]
    private bool _hideByCameraMove = false;
    [SerializeField]
    private bool _hideByCameraZoom = false;
    [SerializeField]
    private bool _hideByMovement = false;
    [SerializeField]
    private bool _hideByJump = false;
    [SerializeField]
    private bool _hideByDeploy = true;
    [SerializeField]
    private bool _followPlayer = true;
    [SerializeField]
    private string _attachPointName = "Tutorial Attach Point";

    public bool IsFollowing { get { return _followPlayer && _prompt.gameObject.activeInHierarchy; } }

    private Collider _collider = null;
    private ScaledOneShotTimer _timer = null;
    private ScaledOneShotTimer _delayTimer = null;
    private CanvasLookAtCamera _prompt = null;
    private Transform _attachPoint = null;
    private bool _shown = false;

    private PlayerMovement _mover = null;
    private PlayerJump _jump = null;
    private DeployControlledBots _deploy = null;
    private BotReleaser _bot = null;

    private HashSet<TutorialActivation> _otherTutorials = null;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _prompt = GetComponentInChildren<CanvasLookAtCamera>();
        _timer = gameObject.AddComponent<ScaledOneShotTimer>();
        _delayTimer = gameObject.AddComponent<ScaledOneShotTimer>();
        _otherTutorials = new HashSet<TutorialActivation>(FindObjectsOfType<TutorialActivation>());
        _otherTutorials.Remove(this);
    }

    private void Start()
    {
        _prompt.gameObject.SetActive(false);
        _delayTimer.OnTimerCompleted += DoneDelay;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null) MyWorkHereIsDone();
        if (_delayTimer != null) _delayTimer.OnTimerCompleted -= DoneDelay;
    }

    private void Update()
    {
        if (GameManager.Instance.GamePaused) return;

        if (!_shown) return;

        if (_followPlayer)
        {
            _prompt.transform.position = _attachPoint.position;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_shown) return;

        if (_followPlayer)
        {
            foreach (TutorialActivation tut in _otherTutorials)
            {
                if (tut.IsFollowing) return;
            }
        }

        PlayerMovement foundMover = other.GetComponent<PlayerMovement>();

        if (foundMover == null || foundMover.ControlsDisabled)
        {
            return;
        }
        else if (_mover == null)
        {
            if (_hideByDeploy)
            {
                _deploy = other.GetComponent<DeployControlledBots>();
                if (_deploy != null && PrefsManager.Instance.BotsUnlocked) _deploy.OnDeployBot += MyWorkHereIsDone;
                else return;
            }
            _mover = foundMover;
        }

        if (!_showWhileInArea)
        {
            _collider.enabled = false;
        }

        if (_followPlayer)
        {
            _attachPoint = other.transform.Find(_attachPointName);
        }

        _prompt.gameObject.SetActive(true);
        _shown = true;

        if (_hideTime > 0.0f)
        {
            _timer.StartTimer(_hideTime);
            _timer.OnTimerCompleted += MyWorkHereIsDone;
        }

        if (_hideByCameraMove || _hideByCameraZoom || _hideByMovement)
        {
            _delayTimer.StartTimer(_delayTime);
        }

        if (_hideByJump)
        {
            _jump = other.GetComponent<PlayerJump>();
            if (_jump != null) _jump.OnPlayerJump += MyWorkHereIsDone;
        }

        _bot = other.GetComponent<BotReleaser>();
        if (_bot != null)
        {
            _bot.OnBotReleased += MyWorkHereIsDone;
        }

        GameManager.Instance.Player.OnPlayerDeath += MyWorkHereIsDone;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_showWhileInArea || _mover == null) return;

        PlayerMovement foundMover = other.GetComponent<PlayerMovement>();
        if (foundMover == null || foundMover != _mover) return;

        _mover = null;
        ElvisHasLeftTheBuilding();
    }

    private void DoneDelay()
    {
        if (_hideByCameraMove)
        {
            GameManager.Instance.Camera.OnCameraMovedByPlayer += MyWorkHereIsDone;
        }

        if (_hideByCameraZoom)
        {
            GameManager.Instance.Camera.OnCameraZoomedByPlayer += MyWorkHereIsDone;
        }

        if (_hideByMovement)
        {
            _mover.OnPlayerMovement += MyWorkHereIsDone;
        }
    }

    private void MyWorkHereIsDone()
    {
        if (!_delayTimer.IsRunning)
        {
            if (_hideByCameraMove && GameManager.Instance.Camera != null) GameManager.Instance.Camera.OnCameraMovedByPlayer -= MyWorkHereIsDone;
            if (_hideByCameraZoom && GameManager.Instance.Camera != null) GameManager.Instance.Camera.OnCameraZoomedByPlayer -= MyWorkHereIsDone;
            if (_hideByMovement && _mover != null) _mover.OnPlayerMovement -= MyWorkHereIsDone;
        }
        else
        {
            _delayTimer.StopTimer();
        }

        if (_hideByJump && _jump != null) _jump.OnPlayerJump -= MyWorkHereIsDone;
        if (_hideByDeploy && _deploy != null) _deploy.OnDeployBot -= MyWorkHereIsDone;
        if (_bot != null) _bot.OnBotReleased -= MyWorkHereIsDone;
        if (GameManager.Instance.Player != null) GameManager.Instance.Player.OnPlayerDeath -= MyWorkHereIsDone;

        _mover = null;

        if (_hideTime > 0.0f)
        {
            _timer.StopTimer();
            _timer.OnTimerCompleted -= MyWorkHereIsDone;
        }

        if (_reshowTime > 0.0f)
        {
            _prompt.gameObject.SetActive(false);
            _timer.OnTimerCompleted += MoreWork;
            _timer.StartTimer(_reshowTime);
        }
        else if (_showWhileInArea)
        {
            ElvisHasLeftTheBuilding();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void MoreWork()
    {
        _timer.OnTimerCompleted -= MoreWork;
        _shown = false;
        _collider.enabled = true;
    }

    private void ElvisHasLeftTheBuilding()
    {
        _shown = false;
        if (_prompt != null) _prompt.gameObject.SetActive(false);
    }
}
