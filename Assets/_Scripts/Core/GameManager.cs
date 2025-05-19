using System;
using System.Collections.Generic;
using UnityEngine;
using YG;
using Core;
using YG.Insides;

namespace Managers
{
    public sealed class GameManager : MonoBehaviourSingleton<GameManager>
    {
        [SerializeField] private SceneController _sceneController;
        [SerializeField] private AdManager _adManager;
        [SerializeField] private LevelLoader _levelLoader;

        public IReadOnlyList<LevelData> Levels { get; private set; }
        public SystemLanguage CurrentLanguage { get; private set; }

        public event Action<SystemLanguage> LanguageChanged;

        protected override void OnAwake()
        {
            CurrentLanguage = Application.systemLanguage;
            _adManager.SetStickyAds(true);

            _levelLoader.Initialize();
            Levels = _levelLoader.Levels;

            YGInsides.LoadProgress();
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused) YG2.SaveProgress();
        }

        private void OnApplicationQuit() => YG2.SaveProgress();

        public void ToggleLanguage()
        {
            CurrentLanguage = CurrentLanguage == SystemLanguage.Russian
                ? SystemLanguage.English
                : SystemLanguage.Russian;
            LanguageChanged?.Invoke(CurrentLanguage);
        }

        public SceneController SceneController => _sceneController;
        public AdManager AdManager => _adManager;
        public LevelLoader LevelLoader => _levelLoader;
    }
}
