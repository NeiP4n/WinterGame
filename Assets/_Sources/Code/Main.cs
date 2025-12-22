using Sources.Code.Gameplay;
// using Sources.Code.Gameplay.AudioSystems;
// using Sources.Code.Particles;
using Sources.Code.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sources.Code
{
    public class Main : MonoBehaviour, IMain
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _particlesParent;
        
        private Game _game;
        
        public Camera Camera => _camera;

        private void Start()
        {
            // var audioSystem = AudioSystem.Instance;
            // audioSystem.Init();
            
            var screenSwitcher = ScreenSwitcher.Instance;
            screenSwitcher.Init();

            // var particlesPlayer = ParticlesPlayer.Instance;
            // particlesPlayer.Init(_particlesParent);
            
            // _game = new Game(this);
            
            screenSwitcher.ShowScreen<MenuScreen>().Init(this);
        }
    
        private void Update()
        {
            _game?.ThisUpdate();

#if (UNITY_EDITOR)
            if (Input.GetKeyDown(KeyCode.R))
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
#endif
        }

        private void OnDisable()
        {
            if (_game == null)
                return;
            
            _game.Dispose();
        }

        public void StartGame()
        {
            _game.StartGame();
        }
    }

    public interface IMain
    {
        public Camera Camera { get; }
        
        public void StartGame();
    }
}