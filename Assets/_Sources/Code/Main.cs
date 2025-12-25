// using Sources.Code.Gameplay.AudioSystems;
// using Sources.Code.Particles;
using Sources.Code.UI;
using Sources.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sources.Code
{
    [DefaultExecutionOrder(-100)]
    public class Main : MonoBehaviour, IMain
    {
        // [SerializeField] private Transform _particlesParent;
        
        private Gameplay.Game _game;
        


        private void Start()
        {
            var inputManager = InputManager.Instance;

            // var audioSystem = AudioSystem.Instance;
            // audioSystem.Init();
            
            var screenSwitcher = ScreenSwitcher.Instance;
            screenSwitcher.Init();

            // var particlesPlayer = ParticlesPlayer.Instance;
            // particlesPlayer.Init(_particlesParent);
            
            _game = new Gameplay.Game(this);
            
            screenSwitcher.ShowScreen<MenuScreen>().Init(this);
            
        }
    
        private void Update()
        {
            _game?.ThisUpdate();

#if (UNITY_EDITOR)
            if (Input.GetKeyDown(KeyCode.R)) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            
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
        // public Camera Camera { get; }
        
        public void StartGame();
    }
}