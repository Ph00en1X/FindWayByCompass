using System.Collections;
using TMPro;
using UnityEngine;
using Managers;
using YG;

public sealed class GameController : MonoBehaviour
{
    [SerializeField] private LevelGenerator _generator;
    [SerializeField] private Compass _compass;
    [SerializeField] private Pointer _pointer;
    [SerializeField] private TMP_Text _timer;
    [SerializeField] private AudioPlayer _windPlayer;
    [SerializeField] private AudioPlayer _clockPlayer;
    [SerializeField] private AudioClip _windClip;
    [SerializeField] private AudioClip _clockClip;
    [SerializeField] private GameObject _winView;
    [SerializeField] private GameObject _loseView;
    [SerializeField] private float _baseTime = 60f;
    [SerializeField] private float _timePerLevel = 10f;

    private PlayerController _playerController;
    private int _level;
    private float _timeLeft;
    private bool _running;

    public bool IsRunning => _running;

    private void Awake()
    {
        _level = YG2.saves.level;
        _windPlayer.Configure(_windClip, true);
        _clockPlayer.Configure(_clockClip, true);
        StartLevel();
    }

    public void RegisterPlayer(GameObject player)
    {
        _playerController = player.GetComponent<PlayerController>();
        _playerController.Initialize(_compass);
        var cd = player.GetComponent<CollisionDispatcher>();
        cd.PortalHit += Win;
        cd.ObstacleHit += _playerController.Stop;
    }

    public void RestartLevel() => StartLevel();

    public void BackToMenu() => GameManager.Instance.SceneController.Load("MenuScene");

    private void StartLevel()
    {
        _running = true;
        _winView.SetActive(false);
        _loseView.SetActive(false);
        _compass.Rotate();
        _windPlayer.Play();

        _timeLeft = _baseTime + _timePerLevel * _level;
        _generator.Build(GameManager.Instance.Levels[_level]);
        UpdateTimer();
        _pointer.Initialize();
        StartCoroutine(Tick());
    }

    private IEnumerator Tick()
    {
        while (_timeLeft > 0 && _running)
        {
            _timeLeft -= Time.deltaTime;
            UpdateTimer();
            if (!_clockPlayer.Playing()) _clockPlayer.Play();
            yield return null;
        }

        if (_running) Lose();
    }

    private void UpdateTimer()
    {
        var m = Mathf.FloorToInt(_timeLeft / 60);
        var s = Mathf.FloorToInt(_timeLeft % 60);
        _timer.text = $"{m:00}:{s:00}";
    }

    private void Win()
    {
        if (!_running) return;

        _level = Mathf.Min(_level + 1, GameManager.Instance.Levels.Count - 1);
        _winView.SetActive(true);
        EndGame();
    }

    private void Lose()
    {
        if (!_running) return;

        _level = Mathf.Max(_level - 1, 0);
        _loseView.SetActive(true);
        _playerController = null;
        EndGame();
    }

    private void EndGame()
    {
        _running = false;

        _clockPlayer.Stop();

        YG2.saves.level = _level;
        Debug.Log("Save level");

        GameManager.Instance.AdManager.Interstitial();
    }
}
