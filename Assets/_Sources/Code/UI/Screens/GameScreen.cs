using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Sources.Code.UI
{
    public class GameScreen : BaseScreen
    {
        [SerializeField] private Image _image;
        
        private Color _hideScreenColor = Color.black;
        
        public void Init()
        {
            _image.color = _hideScreenColor;
            _image.DOFade(0, 2).SetEase(Ease.InCubic);
        }
    }
}