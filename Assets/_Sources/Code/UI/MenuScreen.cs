using UnityEngine;
using UnityEngine.UI;

namespace Sources.Code.UI
{
    public class MenuScreen : BaseScreen
    {
        [SerializeField] private Button _playButton;
    
        private IMain _main;
    
        public void Init(IMain main)
        {
            _main = main;
        }

        private void OnEnable()
        {
            _playButton.onClick.AddListener(OnClickedPlayButton);
        }

        private void OnDisable()
        {
            _playButton.onClick.RemoveListener(OnClickedPlayButton);
        }

        private void OnClickedPlayButton()
        {
            _main.StartGame();
        }
    }
}
