using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum PlayerStateMode
{
    IDLE,
    ROLL,
    SPRINT
}

public class PlayerStateMachine : MonoBehaviour
{
    #region Exposed

    [Header("Timer Parameters")]
    [SerializeField] private float _rollDuration = 0.5f;
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _sprintSpeed = 4f;
    [SerializeField] private float _rollingSpeed = 5f;
    [SerializeField] private AnimationCurve _rollCurve;
    [SerializeField] private float _rollSpeedMultiplier = 100f;
    #endregion

    #region Unity LifeCycle
    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _rb2D = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        TransitionToState(PlayerStateMode.IDLE);
    }

    void Update()
    {
        OnStateUpdate();
        SetInput();
    }
    private void FixedUpdate()
    {
        _rb2D.velocity = _direction.normalized * _currentSpeed * Time.fixedDeltaTime * 50;
    }
    #endregion

    #region Methods

    void SetInput()
    {
        _direction.x = Input.GetAxisRaw("Horizontal");
        _direction.y = Input.GetAxisRaw("Vertical");
    }

    void OnStateEnter() 
    {
        switch (_currentState)
        {
            case PlayerStateMode.IDLE:
                break;
            case PlayerStateMode.ROLL:
                _animator.SetBool("isRolling", true);
                //incrémentation du timer
                _endRollTime = Time.timeSinceLevelLoad + _rollDuration;
                break;
            case PlayerStateMode.SPRINT:
                _animator.SetBool("isSprinting", true);
                break;
            default:
                break;
        }
    }
    void OnStateUpdate()
    {
        switch (_currentState)
        {
            case PlayerStateMode.IDLE:
                _currentSpeed = _moveSpeed;
                _animator.SetFloat("DirectionX", _direction.x);
                _animator.SetFloat("DirectionY", _direction.y);
                if (Input.GetButtonDown("Fire3"))
                {
                    TransitionToState(PlayerStateMode.ROLL);
                }
              //if (Input.GetButton("Fire3") && Time.timeSinceLevelLoad > _endRollTime)
              //{
              //    TransitionToState(PlayerStateMode.SPRINT);
              //}
                break;
            case PlayerStateMode.ROLL:
                
                _rollCount += Time.deltaTime;

                _rollingSpeed = _rollCurve.Evaluate(_rollCount / _rollDuration) * _rollSpeedMultiplier;
                _currentSpeed = _rollingSpeed;
                //Condition du timer
                if (Time.timeSinceLevelLoad > _endRollTime)
                {
                    if (Input.GetButton("Fire3"))
                    {
                        TransitionToState(PlayerStateMode.SPRINT);
                    }
                    else
                    {
                        TransitionToState(PlayerStateMode.IDLE);

                    }
                }
                break;
            case PlayerStateMode.SPRINT:
                _currentSpeed = _sprintSpeed;
                _animator.SetFloat("DirectionX", _direction.x);
                _animator.SetFloat("DirectionY", _direction.y);
                if (Input.GetButtonUp("Fire3"))
                {
                    TransitionToState(PlayerStateMode.IDLE);
                }
                break;
            default:
                break;
        }
    }

    void OnStateExit()
    {
        switch (_currentState)
        {
            case PlayerStateMode.IDLE:
                break;
            case PlayerStateMode.ROLL:
                _rollCount = 0;
                _animator.SetBool("isRolling", false);
                break;
            case PlayerStateMode.SPRINT:
                _animator.SetBool("isSprinting", false);
                break;
            default:
                break;
        }
    }

    void TransitionToState(PlayerStateMode toState)
    {
        OnStateExit();
        _currentState = toState;
        OnStateEnter();
    }

    #endregion

    #region Private & Protected

    private PlayerStateMode _currentState;
    private Animator _animator;
    private float _endRollTime;
    private Rigidbody2D _rb2D;
    private Vector2 _direction;
    private float _currentSpeed;
    private float _rollCount;


    #endregion
}
