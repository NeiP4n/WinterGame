using DG.Tweening;
using Game.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Sources.Code.UI
{
    public class GameScreen : BaseScreen
    {
        [SerializeField] private Image _image;
        [SerializeField] private UIInteract _uiInteract;
        
        private Color _hideScreenColor = Color.black;
        
        public void Init()
        {
            _image.color = _hideScreenColor;
            _image.DOFade(0, 2).SetEase(Ease.InCubic);
        }
        public UIInteract GetUIInteract()
        {
            return _uiInteract;
        }
    }
}