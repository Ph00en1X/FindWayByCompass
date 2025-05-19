using UnityEngine;

public sealed class Pointer : MonoBehaviour
{
    private Transform _player;
    private Transform _portal;
    private GameController _controller;

    public void Initialize()
    {
        _controller = FindAnyObjectByType<GameController>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _portal = GameObject.FindGameObjectWithTag("Portal").transform;
    }

    private void FixedUpdate()
    {
        if (!_player || !_portal) return;
        if (!_controller.IsRunning) return;

        var dir = (_player.position - _portal.position).normalized;
        var angle = Mathf.RoundToInt(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
