using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class PlayerController : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private Animator _animator;
    [SerializeField] private AudioPlayer _steps;
    [SerializeField] private AudioPlayer _actions;
    [SerializeField] private AudioClip _clickClip;
    [SerializeField] private AudioClip _stepClip;
    [SerializeField] private AudioClip _breathClip;

    private GameController _controller;
    private Compass _compass;
    private Rigidbody2D _rb;
    private Vector3 _originScale;

    public void Initialize(Compass compass)
    {
        _compass = compass;
        _rb = GetComponent<Rigidbody2D>();
        _originScale = transform.localScale;
        _actions.Configure(_clickClip);
        _controller = FindAnyObjectByType<GameController>();
        FindAnyObjectByType<CinemachineCamera>()
            .GetComponent<PlayerCamera>().Init(transform);
    }

    private void Update()
    {
        if (!_controller.IsRunning) Stop();
        if (!InputReceived()) return;

        _actions.Play();
        var dir = DirFromAngle(_compass.Angle);
        _rb.linearVelocity = dir * _speed;
        UpdateAnimation(dir);
        UpdateSound();
    }

    public void Stop()
    {
        _rb.linearVelocity = Vector2.zero;
        UpdateAnimation(Vector2.zero);
        UpdateSound();
    }

    private void UpdateAnimation(Vector2 dir)
    {
        transform.localScale = dir.x < 0 ? new Vector3(-_originScale.x, _originScale.y) : _originScale;

        if (!_animator) return;
        _animator.SetFloat("X", dir.x);
        _animator.SetFloat("Y", dir.y);
    }

    private void UpdateSound()
    {
        if (_controller.IsRunning)
        {
var moving = _rb.linearVelocity.sqrMagnitude > 0.01f;
        _steps.Configure(moving ? _stepClip : _breathClip, true, moving ? .2f : 1f);
        if (!_steps.Playing()) _steps.Play();
        }
        else
        {
            _steps.Stop();

        }
    }

    private static bool InputReceived() =>
        Touchscreen.current?.primaryTouch.press.wasPressedThisFrame == true ||
        Mouse.current?.leftButton.wasPressedThisFrame == true ||
        Keyboard.current?.spaceKey.wasPressedThisFrame == true;

    private static Vector2 DirFromAngle(float angle) =>
        new(-Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad));
}
