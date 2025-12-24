using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Interfaces;
using Game.Managers;
using Sources.Code.Configs;
using Sources.Code.Gameplay.GameSaves;
using Sources.Code.UI;
using UnityEngine;

namespace Sources.Code.Gameplay
{
    public class Game
    {
        private readonly PlayerProgress _playerProgress;
        private readonly ScreenSwitcher _screenSwitcher;
        private readonly PopupSwitcher _popupSwitcher;
        private readonly IMain _main;
        private readonly List<IMonoBehaviour> _monoBehaviours = new();
        private readonly GameTokenProvider _tokenProvider;
        private readonly LevelsConfig _levelsConfig;
        private readonly IInputManager _inputManager;

        private CancellationTokenSource _gameTokenSource;
        private CancellationToken _gameToken;

        private GameEventPopup _gameEventScreen;
        private Level _levelInstance;
        private PlayerCharacter _playerCharacter;
        private bool _isWin;

        public int CurrentLevelNumber
        {
            get => _playerProgress.LevelNumber;
            set => _playerProgress.LevelNumber = value;
        }

        public int MaxLevels => _levelsConfig.LevelCount;

        public Game(IMain main)
        {
            _main = main;
            _tokenProvider = GameTokenProvider.Instance;
            _playerProgress = GameSaverLoader.Instance.PlayerProgress;
            _screenSwitcher = ScreenSwitcher.Instance;
            _popupSwitcher = PopupSwitcher.Instance;
            _levelsConfig = LevelsConfig.Instance;
            _inputManager = InputManager.Instance;
        }

        public void ThisUpdate()
        {
            if (_screenSwitcher.ScreenIsActive<GameScreen>(out _) && Input.GetKeyDown(KeyCode.Escape))
            {
                ReleaseCursor();

                var menu = _screenSwitcher.ShowScreen<MenuScreen>();
                menu.Init(_main);
                return;
            }

            foreach (var mono in _monoBehaviours)
                mono.Tick();
        }


        public void StartGame()
        {
            _popupSwitcher.Init();

            _gameTokenSource = new CancellationTokenSource();
            _gameToken = _gameTokenSource.Token;
            _tokenProvider.Init(_gameToken);

            int index = CurrentLevelNumber - 1;
            _levelInstance = Object.Instantiate(_levelsConfig.GetLevelPrefabByIndex(index));

            _playerCharacter = _levelInstance.PlayerCharacter;
            _playerCharacter.Construct(_inputManager);

            if (_playerCharacter is IMonoBehaviour mono)
                _monoBehaviours.Add(mono);
            
            ApplyGameplayCursor();

            _screenSwitcher.ShowScreen<GameScreen>().Init();
        }




        public void Dispose()
        {
            ClearLevel();

            if (_gameTokenSource != null)
            {
                _gameTokenSource.Cancel();
                _gameTokenSource.Dispose();
                _gameTokenSource = null;
            }
        }

        private void RestartLevel()
        {
            ClearLevel();
            _isWin = false;
            StartGame();
        }

        private async UniTaskVoid DefeatLevel()
        {
            if (_isWin)
                return;

            _gameEventScreen = _popupSwitcher.GetPopup<GameEventPopup>();
            _gameEventScreen.Init();
            _gameEventScreen.ShowDefeat();

            await UniTask.Delay(1500, cancellationToken: _gameToken);

            _gameEventScreen.Destroy();
            RestartLevel();
        }

        private void ClearLevel()
        {
            foreach (var mono in _monoBehaviours)
                mono.Dispose();

            _monoBehaviours.Clear();

            if (_gameEventScreen != null)
            {
                _gameEventScreen.Destroy();
                _gameEventScreen = null;
            }

            if (_levelInstance != null)
            {
                Object.Destroy(_levelInstance.gameObject);
                _levelInstance = null;
            }
        }
        private void ApplyGameplayCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        private void ReleaseCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
