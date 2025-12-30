using UnityEngine;
using UnityEngine.UI;

namespace Sources.Code.UI
{
    public class MenuScreen : BaseScreen
    {
        [Header("Main Menu")]
        [SerializeField] private CanvasGroup _mainGroup;
        [SerializeField] private Button _playButton;
        private IMain _main;

        public void Init(IMain main)
        {
            _main = main;
            ShowMain();
        }

        private void OnEnable()
        {
            _playButton.onClick.AddListener(OnPlayClicked);
        }

        private void OnDisable()
        {
            _playButton.onClick.RemoveListener(OnPlayClicked);
        }

        private void OnPlayClicked()
        {
            DisableAllInput();
            _main.StartGame();
        }


        private void ShowMain()
        {
            SetGroup(_mainGroup, true);
        }

        private void DisableAllInput()
        {
            SetGroup(_mainGroup, false);
        }

        private static void SetGroup(CanvasGroup group, bool visible)
        {
            if (group == null) return;
            group.alpha = visible ? 1f : 0f;
            group.interactable = visible;
            group.blocksRaycasts = visible;
        }
    }
}
