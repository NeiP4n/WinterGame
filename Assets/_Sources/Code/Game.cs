using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sources.Code.Configs;
// using Sources.Code.Gameplay.AudioSystems;
// using Sources.Code.Gameplay.DoorRoot;
using Sources.Code.Gameplay.GameSaves;
// using Sources.Code.Gameplay.Jails;
// using Sources.Code.Gameplay.PlayerRoot;
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
        private readonly List<IMonoBehaviour> _monoBehaviours = new List<IMonoBehaviour>();
        private readonly GameTokenProvider _tokenProvider;
        private readonly LevelsConfig _levelsConfig;
        // private readonly MusicSystem _musicSystem;
        
        private CancellationTokenSource _gameTokenSource = new();
        private GameEventPopup _gameEventScreen;
        private CancellationToken _gameToken;
        private Level _levelInstance;
        // private PlayerCharacter _playerCharacter;
        // private Player _player;
        private bool _isWin;
        
        public int CurrentLevelNumber
        {
            get => _playerProgress.LevelNumber;
            set => _playerProgress.LevelNumber = value;
        }
    
        public int MaxLevels => _levelsConfig.LevelCount;
        
        public Game(IMain main)
        {
            _tokenProvider = GameTokenProvider.Instance;
            _playerProgress = GameSaverLoader.Instance.PlayerProgress;
            _screenSwitcher = ScreenSwitcher.Instance;
            _popupSwitcher = PopupSwitcher.Instance;
            _levelsConfig = LevelsConfig.Instance;
            // _musicSystem = new MusicSystem();
            _main = main;
        }
        
        public void ThisUpdate()
        {
            if (_screenSwitcher.ScreenIsActive<GameScreen>(out _) && Input.GetKeyDown(KeyCode.Escape))
            {
                //ClearLevel();
                var menuScreen = _screenSwitcher.ShowScreen<MenuScreen>();
                menuScreen.Init(_main);
            }

            foreach (var monoBehaviour in _monoBehaviours)
                monoBehaviour.Tick();
            
#if (UNITY_EDITOR)
            if (Input.GetKeyDown(KeyCode.F))
                ClearSaves();

            if (Input.GetKeyDown(KeyCode.G))
                OnNextLevel();
#endif
        }

        public void Dispose()
        {
            foreach (var monoBehaviour in _monoBehaviours)
                monoBehaviour.Dispose();
            
            if (_gameTokenSource.IsCancellationRequested)
                return;
                                    
            _gameTokenSource.Cancel();
            _gameTokenSource.Dispose();
        }

        public void StartGame()
        {
            _popupSwitcher.Init();
            _gameTokenSource = new CancellationTokenSource();
            _gameToken = _gameTokenSource.Token;
            _tokenProvider.Init(_gameTokenSource.Token);
            
            int levelIndex = CurrentLevelNumber - 1;
            _levelInstance = GameObject.Instantiate(_levelsConfig.GetLevelPrefabByIndex(levelIndex));
            
            _screenSwitcher.ShowScreen<GameScreen>().Init();
        }
        
        private void OnDied(Vector2 pos)
        {
            DefeatLevel().Forget();
        }
        
        private void OnNextLevel()
        {
            if (CurrentLevelNumber == MaxLevels)
            {
                _isWin = true;
                _gameEventScreen = _popupSwitcher.GetPopup<GameEventPopup>();
                _gameEventScreen.Init();
                _gameEventScreen.ShowVictory();
                return;
            }
            
            CurrentLevelNumber++;
            RestartLevel();
        }
        
        private async UniTaskVoid DefeatLevel()
        {
            if (_isWin)
                return;
            
            _gameEventScreen = _popupSwitcher.GetPopup<GameEventPopup>();
            _gameEventScreen.Init();
            _gameEventScreen.ShowDefeat();

            await UniTask.WaitForSeconds(1.5f, cancellationToken: _gameToken);
                            
            _gameEventScreen.Destroy();
            RestartLevel();
        }

        private void ClearLevel()
        {
            if (_levelInstance == null)
                return;
            
            _monoBehaviours.Clear();
            Dispose();
            _levelInstance = null;
        }
        
        private void RestartLevel()
        {
            ClearLevel();
            StartGame();
        }
        
        private void ClearSaves()
        {
            CurrentLevelNumber = 1;
        }
    }
}