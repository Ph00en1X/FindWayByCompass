using DG.Tweening;
using UnityEngine;

public sealed class Compass : MonoBehaviour
{
    [SerializeField] private RectTransform _arrow;
    [SerializeField] private float _stepSeconds = 1f;

    private GameController _controller;
    private float _angle;
    private readonly System.Random _rng = new System.Random();
    private Tween _tween;

    public float Angle => transform.rotation.eulerAngles.z;

    private void Awake() => _controller = FindAnyObjectByType<GameController>();
    private void OnDisable() => _tween?.Kill();

    public void Rotate()
    {
        if (_controller.IsRunning)
        {
            _angle = _rng.Next(0, 360);
            _tween = _arrow.DORotate(new Vector3(0, 0, _angle), _stepSeconds, RotateMode.Fast)
                           .SetEase(Ease.OutQuad)
                           .OnComplete(Rotate);
        }
    }
}